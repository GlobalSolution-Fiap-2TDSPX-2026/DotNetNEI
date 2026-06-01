using System.Text.Json;
using NEI.Data;
using NEI.Models;
using NEI.DTOs;

namespace NEI.Services
{
    public class NasaIntegrationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        private readonly RiskAssessmentService _riskAssessmentService;

        public NasaIntegrationService(IHttpClientFactory httpClientFactory, IConfiguration configuration, AppDbContext dbContext, RiskAssessmentService riskAssessmentService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _dbContext = dbContext;
            _riskAssessmentService = riskAssessmentService;

        }

        public async Task SyncAsteroidsAsync(DateTime startDate, DateTime endDate)
        {
            var client = _httpClientFactory.CreateClient("NasaClient");
            var apiKey = _configuration["NasaApi:ApiKey"];
            var start = startDate.ToString("yyyy-MM-dd");
            var end = endDate.ToString("yyyy-MM-dd");

            // requisição
            var response = await client.GetAsync($"feed?start_date={start}&end_date={end}&api_key={apiKey}");
            response.EnsureSuccessStatusCode();

            // reesposta como texto puro
            var jsonString = await response.Content.ReadAsStringAsync();
            // pega o texto puro e joga dentro das classes DTOs para ser um objeto C#.
            var feedData = JsonSerializer.Deserialize<NasaFeedResponse>(jsonString);

            if (feedData?.NearEarthObjects == null) return;

            // O JSON da NASA agrupa os asteroides por dia. Primeiro olha para os dias, depois para os asteroides dentro daquele dia.
            foreach (var dateKey in feedData.NearEarthObjects.Keys)
            {
                foreach (var neo in feedData.NearEarthObjects[dateKey])
                {
                    // 1. Verifica se o asteroide já existe usando o índice único
                    var asteroid = _dbContext.Asteroids.FirstOrDefault(a => a.NasaId == neo.Id);
                    
                    if (asteroid == null)
                    {
                        asteroid = new Asteroid
                        {
                            NasaId = neo.Id,
                            Name = neo.Name,
                            AbsoluteMagnitude = (decimal)neo.AbsoluteMagnitude,
                            EstimatedDiameterMinKm = neo.EstimatedDiameter.Kilometers.Min,
                            EstimatedDiameterMaxKm = neo.EstimatedDiameter.Kilometers.Max,
                            IsPotentiallyDangerous = neo.IsPotentiallyHazardous
                        };
                        _dbContext.Asteroids.Add(asteroid);
                        await _dbContext.SaveChangesAsync(); 
                    }

                    // 2. Registra as aproximações e gera as Avaliações de Risco
                    foreach (var approach in neo.CloseApproachData)
                    {
                        var approachDate = DateTime.Parse(approach.CloseApproachDate);
                        decimal missDistance = decimal.Parse(approach.MissDistance.Kilometers, 
                                                            System.Globalization.CultureInfo.InvariantCulture);

                        decimal relativeVelocity = decimal.Parse(approach.RelativeVelocity.KilometersPerSecond, 
                                                                System.Globalization.CultureInfo.InvariantCulture);
                        
                        // Evita duplicar a mesma aproximação
                        if (!_dbContext.CloseApproaches.Any(c => c.AsteroidId == asteroid.Id && c.ApproachDate == approachDate))
                        {
                            _dbContext.CloseApproaches.Add(new CloseApproach
                            {
                                AsteroidId = asteroid.Id,
                                ApproachDate = approachDate,
                                MissDistanceKm = decimal.Parse(approach.MissDistance.Kilometers, System.Globalization.CultureInfo.InvariantCulture),
                                RelativeVelocityKm = decimal.Parse(approach.RelativeVelocity.KilometersPerSecond, System.Globalization.CultureInfo.InvariantCulture),
                                OrbitingBody = approach.OrbitingBody
                            });
                        }
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
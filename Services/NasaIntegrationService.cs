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

        public NasaIntegrationService(IHttpClientFactory httpClientFactory, IConfiguration configuration, AppDbContext dbContext)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _dbContext = dbContext;
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
                        // ====================================================================
                        // MOTOR DE RISCO
                        // ====================================================================
                        
                        // Limite de segurança: 7.500.000 km (aproximadamente 0.05 AU - Padrão NASA)
                        decimal safeDistanceThreshold = 7500000m;
                        RiskLevel calculatedRiskLevel = RiskLevel.LOW;

                        // Regra de classificação
                        if (neo.IsPotentiallyHazardous && missDistance <= safeDistanceThreshold)
                            calculatedRiskLevel = RiskLevel.CRITICAL;
                        else if (!neo.IsPotentiallyHazardous && missDistance <= safeDistanceThreshold)
                            calculatedRiskLevel = RiskLevel.HIGH;
                        else if (neo.IsPotentiallyHazardous && missDistance > safeDistanceThreshold)
                            calculatedRiskLevel = RiskLevel.MEDIUM;

                        // Verifica se já existe uma avaliação de risco para essa mesma data e asteroide
                        var existingAssessment = _dbContext.RiskAssessments
                            .FirstOrDefault(ra => ra.AsteroidId == asteroid.Id && ra.AssessedAt == approachDate);

                        if (existingAssessment == null)
                        {
                            var riskAssessment = new RiskAssessment
                            {
                                AsteroidId = asteroid.Id,
                                RiskLevel = calculatedRiskLevel,
                                MissDistanceKm = missDistance,
                                SafeDistanceThresholdKm = safeDistanceThreshold,
                                AssessedAt = approachDate // A data da aproximação será a data da avaliação de risco
                            };

                            _dbContext.RiskAssessments.Add(riskAssessment);
                            
                            // Salva no banco
                            await _dbContext.SaveChangesAsync(); 

                            // Se o risco for Alto ou Crítico, geramos uma Zona de Risco para o Dashboard
                            if (calculatedRiskLevel == RiskLevel.CRITICAL || calculatedRiskLevel == RiskLevel.HIGH)
                            {
                                var riskZone = new RiskZone
                                {
                                    RiskAssessmentId = riskAssessment.Id,
                                    RegionName = "Zona de Impacto Estimada (Simulação)",
                                    // Gera coordenadas aleatórias usando Random.Shared do .NET para simular o ponto de queda
                                    Latitude = (decimal)(Random.Shared.NextDouble() * 180 - 90), 
                                    Longitude = (decimal)(Random.Shared.NextDouble() * 360 - 180),
                                    RadiusKm = calculatedRiskLevel == RiskLevel.CRITICAL ? 1000m : 500m, // Raio de destruição
                                    AlertLevel = calculatedRiskLevel == RiskLevel.CRITICAL ? AlertLevel.RED : AlertLevel.ORANGE
                                };

                                _dbContext.RiskZones.Add(riskZone);
                                await _dbContext.SaveChangesAsync(); // Persiste a zona no banco
                            }
                        }
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
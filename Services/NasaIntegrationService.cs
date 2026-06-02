using System.Globalization;
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
        private readonly CloseApproachService _closeApproachService;
        private readonly RiskAssessmentService _riskAssessmentService;

        public NasaIntegrationService(IHttpClientFactory httpClientFactory, IConfiguration configuration, 
                                        AppDbContext dbContext, CloseApproachService closeApproachService,
                                        RiskAssessmentService riskAssessmentService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _dbContext = dbContext;
            _closeApproachService = closeApproachService;
            _riskAssessmentService = riskAssessmentService;
        }

        public async Task SyncAsteroidsAsync(DateTime startDate, DateTime endDate)
        {
            var client = _httpClientFactory.CreateClient("NasaClient");
            var apiKey = _configuration["NasaApi:ApiKey"];
            var start = startDate.ToString("yyyy-MM-dd");
            var end = endDate.ToString("yyyy-MM-dd");

            var response = await client.GetAsync($"feed?start_date={start}&end_date={end}&api_key={apiKey}");
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var feedData = JsonSerializer.Deserialize<NasaFeedResponse>(jsonString);

            if (feedData?.NearEarthObjects == null) return;

            foreach (var dateKey in feedData.NearEarthObjects.Keys)
            {
                foreach (var neo in feedData.NearEarthObjects[dateKey])
                {
                    var asteroid = await UpsertAsteroidAsync(neo);
                    _closeApproachService.SyncCloseApproaches(asteroid, neo);
                    await _riskAssessmentService.SyncRiskAssessmentsAsync(asteroid, neo);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<Asteroid> UpsertAsteroidAsync(NasaAsteroidDto neo)
        {
            var asteroid = _dbContext.Asteroids.FirstOrDefault(a => a.NasaId == neo.Id);

            if (asteroid != null) return asteroid;

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

            return asteroid;
        }


        
    }
}

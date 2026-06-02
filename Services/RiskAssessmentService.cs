using System;
using System.Globalization;
using NEI.Data;
using NEI.DTOs;
using NEI.Models;

namespace NEI;

public class RiskAssessmentService
{

    private readonly AppDbContext _context;

    public RiskAssessmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SyncRiskAssessmentsAsync(Asteroid asteroid, NasaAsteroidDto neo)
    {
        if (neo.CloseApproachData == null) return;

        const decimal safeDistanceThreshold = 7500000m;

        foreach (var approach in neo.CloseApproachData)
        {
            var approachDate = DateTime.Parse(approach.CloseApproachDate);
            var missDistance = decimal.Parse(approach.MissDistance.Kilometers, CultureInfo.InvariantCulture);

            var calculatedRiskLevel = CalculateRiskLevel(neo.IsPotentiallyHazardous, missDistance, safeDistanceThreshold);

            var existingAssessment = _context.RiskAssessments
                .FirstOrDefault(ra => ra.AsteroidId == asteroid.Id && ra.AssessedAt == approachDate);

            if (existingAssessment != null) continue;

            var riskAssessment = new RiskAssessment
            {
                AsteroidId = asteroid.Id,
                RiskLevel = calculatedRiskLevel,
                MissDistanceKm = missDistance,
                SafeDistanceThresholdKm = safeDistanceThreshold,
                AssessedAt = approachDate
            };

            _context.RiskAssessments.Add(riskAssessment);
            await _context.SaveChangesAsync();

            if (calculatedRiskLevel is RiskLevel.CRITICAL or RiskLevel.HIGH)
                await UpdateRiskZonesForAssessmentAsync(riskAssessment, calculatedRiskLevel);
        }
    }

    private static RiskLevel CalculateRiskLevel(bool isPotentiallyHazardous, decimal missDistance, decimal safeDistanceThreshold)
    {
        if (isPotentiallyHazardous && missDistance <= safeDistanceThreshold)
            return RiskLevel.CRITICAL;

        if (!isPotentiallyHazardous && missDistance <= safeDistanceThreshold)
            return RiskLevel.HIGH;

        if (isPotentiallyHazardous && missDistance > safeDistanceThreshold)
            return RiskLevel.MEDIUM;

        return RiskLevel.LOW;
    }

    private async Task UpdateRiskZonesForAssessmentAsync(RiskAssessment riskAssessment, RiskLevel calculatedRiskLevel)
    {
        var zonasMonitoradas = _context.RiskZones.ToList();

        if (!zonasMonitoradas.Any()) return;

        var zonaAfetada = zonasMonitoradas[Random.Shared.Next(zonasMonitoradas.Count)];

        zonaAfetada.RiskAssessmentId = riskAssessment.Id;
        zonaAfetada.AlertLevel = calculatedRiskLevel == RiskLevel.CRITICAL ? AlertLevel.RED : AlertLevel.ORANGE;
        zonaAfetada.RadiusKm = calculatedRiskLevel == RiskLevel.CRITICAL ? 1000m : 500m;

        _context.RiskZones.Update(zonaAfetada);
        await _context.SaveChangesAsync();
    }

}

namespace NEI;

public record class RiskAssessmentRequest(
    int AsteroidId,
    RiskLevel RiskLevel,
    decimal MissDistanceKm,
    decimal SafeDistanceThresholdKm,
    DateTime AssessedAt
)
{
    public RiskAssessment ToEntity()
    {
        return new RiskAssessment
        {
            AsteroidId = AsteroidId,
            RiskLevel = RiskLevel,
            MissDistanceKm = MissDistanceKm,
            SafeDistanceThresholdKm = SafeDistanceThresholdKm,
            AssessedAt = AssessedAt
        };
    }
}

namespace NEI;

public record class CloseApproachRequest(
    int AsteroidId,
    DateTime ApproachDate,
    decimal MissDistanceKm,
    decimal RelativeVelocityKm,
    string OrbitingBody
)
{
    public CloseApproach ToEntity()
    {
        return new CloseApproach
        {
            AsteroidId = AsteroidId,
            ApproachDate = ApproachDate,
            MissDistanceKm = MissDistanceKm,
            RelativeVelocityKm = RelativeVelocityKm,
            OrbitingBody = OrbitingBody
        };
    }
}

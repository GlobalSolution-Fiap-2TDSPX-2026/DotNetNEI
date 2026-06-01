namespace NEI;

public record class RiskZoneRequest(
    string RegionName,
    decimal Latitude,
    decimal Longitude
)
{

    public RiskZone ToEntity()
    {
        return new RiskZone
        {
            RegionName=this.RegionName,
            Latitude=this.Latitude,
            Longitude=this.Longitude
        };
    }

}

using System.Text.Json.Serialization;

namespace NEI.DTOs
{
    public class NasaFeedResponse
    {
        [JsonPropertyName("near_earth_objects")]
        public Dictionary<string, List<NasaAsteroidDto>> NearEarthObjects { get; set; }
    }

    public class NasaAsteroidDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } // Vai para o seu NasaId

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("absolute_magnitude_h")]
        public double AbsoluteMagnitude { get; set; }

        [JsonPropertyName("estimated_diameter")]
        public EstimatedDiameterDto EstimatedDiameter { get; set; }

        [JsonPropertyName("is_potentially_hazardous_asteroid")]
        public bool IsPotentiallyHazardous { get; set; }

        [JsonPropertyName("close_approach_data")]
        public List<CloseApproachDto> CloseApproachData { get; set; }
    }

    // Classes auxiliares para o JSON aninhado
    public class EstimatedDiameterDto
    {
        [JsonPropertyName("kilometers")]
        public KilometersDto Kilometers { get; set; }
    }

    public class KilometersDto
    {
        [JsonPropertyName("estimated_diameter_min")]
        public decimal Min { get; set; }

        [JsonPropertyName("estimated_diameter_max")]
        public decimal Max { get; set; }
    }

    public class CloseApproachDto
    {
        [JsonPropertyName("close_approach_date")]
        public string CloseApproachDate { get; set; }

        [JsonPropertyName("relative_velocity")]
        public RelativeVelocityDto RelativeVelocity { get; set; }

        [JsonPropertyName("miss_distance")]
        public MissDistanceDto MissDistance { get; set; }

        [JsonPropertyName("orbiting_body")]
        public string OrbitingBody { get; set; }
    }

    public class RelativeVelocityDto
    {
        [JsonPropertyName("kilometers_per_second")]
        public string KilometersPerSecond { get; set; }
    }

    public class MissDistanceDto
    {
        [JsonPropertyName("kilometers")]
        public string Kilometers { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using NEI.Models;

namespace NEI;

[Table("DB_CLOSE_APPROACH")]
public class CloseApproach
{

    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [Column("ASTEROID_ID")]
    public int AsteroidId { get; set; }

    [JsonIgnore]
    [ForeignKey("AsteroidId")]
    public Asteroid Asteroid { get; set; }

    [Required]
    [Column("APPROACH_DATE")]
    public DateTime ApproachDate { get; set; }

    [Required]
    [Column("MISS_DISTANCE_KM", TypeName = "NUMBER(18,2)")]
    public decimal MissDistanceKm { get; set; }

    [Required]
    [Column("RELATIVE_VELOCITY_KM", TypeName = "NUMBER(18,2)")]
    public decimal RelativeVelocityKm { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("ORBITING_BODY")]
    public string OrbitingBody { get; set; }

    public void Update(
        int asteroidId,
        DateTime approachDate,
        decimal missDistanceKm,
        decimal relativeVelocityKm,
        string orbitingBody)
    {
        AsteroidId = asteroidId;
        ApproachDate = approachDate;
        MissDistanceKm = missDistanceKm;
        RelativeVelocityKm = relativeVelocityKm;
        OrbitingBody = orbitingBody;
    }

}

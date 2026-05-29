using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using NEI.Models;

namespace NEI;

[Table("DB_RISK_ASSESSMENTS")]
public class RiskAssessment
{

    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [ForeignKey("ASTEROID_ID")]
    public int AsteroidId { get; set; }

    [JsonIgnore]
    [ForeignKey("AsteroidId")]
    public Asteroid Asteroid { get; set; }

    [Required]
    [Column("RISK_LEVEL")]
    public RiskLevel RiskLevel { get; set; }

    [Required]
    [Column("MISS_DISTANCE_KM", TypeName = "NUMBER(18,2)")]
    public decimal MissDistanceKm { get; set; }

    [Required]
    [Column("SAFE_DISTANCE_THRESHOLD_KM", TypeName = "NUMBER(18,2)")]
    public decimal SafeDistanceThresholdKm { get; set; }

    [Required]
    [Column("ASSESSED_AT")]
    public DateTime AssessedAt { get; set; }

}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NEI;

[Table("DB_RISK_ZONE")]
public class RiskZone
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [Column("RISK_ASSESSMENT_ID")]
    public int RiskAssessmentId { get; set; }

    [JsonIgnore]
    [ForeignKey("RiskAssessmentId")]
    public RiskAssessment RiskAssessment { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("REGION_NAME")]
    public string RegionName { get; set; }

    [Required]
    [Column("LATITUDE", TypeName = "NUMBER(5,2)")]
    public decimal Latitude { get; set; }

    [Required]
    [Column("LONGITUDE", TypeName = "NUMBER(5,2)")]
    public decimal Longitude { get; set; }

    [Required]
    [Column("RADIUS_KM", TypeName = "NUMBER(18,2)")]
    public decimal RadiusKm { get; set; }

    [Required]
    [Column("ALERT_LEVEL", TypeName = "VARCHAR2(6)")]
    public AlertLevel AlertLevel { get; set; }
}

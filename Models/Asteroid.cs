using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEI.Models
{
    [Table("DB_ASTEROIDS")]
    [Index(nameof(NasaId), IsUnique = true)]
    public class Asteroid
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "NasaId is required.")]
        [MaxLength(100)]
        [Column("NASA_ID")]
        public string NasaId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100)]
        [Column("NAME")]
        public string Name { get; set; }

        [Required(ErrorMessage = "EstimatedDiameterMinKm is required.")]
        [Column("ESTIMATED_DIAMETER_MIN_KM", TypeName = "NUMBER(18,2)")]
        public decimal EstimatedDiameterMinKm { get; set; }

        [Required(ErrorMessage = "EstimatedDiameterMaxKm is required.")]
        [Column("ESTIMATED_DIAMETER_MAX_KM", TypeName = "NUMBER(18,2)")]
        public decimal EstimatedDiameterMaxKm { get; set; }

        [Required(ErrorMessage = "AbsoluteMagnitude is required.")]
        [Column("ABSOLUTE_MAGNITUDE", TypeName = "NUMBER(18,2)")]
        public decimal AbsoluteMagnitude { get; set; }

        [Required(ErrorMessage = "IsPotentiallyDangerous is required.")]
        [Column("IS_POTENTIALLY_DANGEROUS")]
        public bool IsPotentiallyDangerous { get; set; }
    }
}

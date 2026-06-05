using Microsoft.EntityFrameworkCore;
using NEI.Models;

namespace NEI.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Asteroid> Asteroids { get; set; }
        public DbSet<CloseApproach> CloseApproaches { get; set; }
        public DbSet<RiskAssessment> RiskAssessments { get; set; }
        public DbSet<RiskZone> RiskZones { get; set; }
        public DbSet<User> Users { get; set; }

    }
}

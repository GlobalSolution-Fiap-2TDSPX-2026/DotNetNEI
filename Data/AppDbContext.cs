using Microsoft.EntityFrameworkCore;

namespace NEI.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }



    }
}

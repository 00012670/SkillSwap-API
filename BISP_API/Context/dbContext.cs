using BISP_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BISP_API.Context
{
    public class dbContext : DbContext
    {
        public dbContext(DbContextOptions<dbContext> o) : base(o)
        {

        }

        public DbSet<Authentication> Authentications {get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Authentication>().ToTable("authentications");
        }
    }
}

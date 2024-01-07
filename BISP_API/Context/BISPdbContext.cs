using BISP_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BISP_API.Context
{
    public class BISPdbContext : DbContext
    {
        public BISPdbContext(DbContextOptions<BISPdbContext> o) : base(o)
        {
            Database.EnsureCreated();
        }

        public DbSet<Authentication> Authentications { get; set; }
        public DbSet<Skill> Skills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Authentication>().ToTable("authentications");
        }
    }
}

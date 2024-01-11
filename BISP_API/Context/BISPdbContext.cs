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

        public DbSet<User> Users { get; set; }
        public DbSet<Skill> Profiles { get; set; }
    //    public DbSet<Profile> Profiles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("authentications");
        }
    }
}

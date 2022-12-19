using Microsoft.EntityFrameworkCore;
using RestoSpotAPI.Models;

namespace RestoSpotAPI.Data
{
    public class RestoSpotDbContext : DbContext
    {
        public RestoSpotDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<restaurants> restaurants { get; set; }
        public DbSet<cuisine> cuisine { get; set; }
        public DbSet<city> city { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
        }
    }
}

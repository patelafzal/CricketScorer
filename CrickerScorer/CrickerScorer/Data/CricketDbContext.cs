using Microsoft.EntityFrameworkCore;
using CrickerScorer.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CrickerScorer.Data
{
    public class CricketDbContext : DbContext
    {
        public CricketDbContext(DbContextOptions<CricketDbContext> options)
            : base(options)
        {
        }

        public DbSet<BallAction> BallActions { get; set; }

        // Optionally override OnModelCreating to configure entity properties.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BallAction>(entity =>
            {
                entity.HasKey(e => e.Id);
                // You can add further configuration or indexes here.
            });
        }
    }
}

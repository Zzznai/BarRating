using BarRating.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BarRating.Data
{
    public class BarRatingDbContext : IdentityDbContext<ApplicationUser>
    {
        public BarRatingDbContext(DbContextOptions<BarRatingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bar> Bars { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customize the ApplicationUser entity
            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.Email).IsRequired(false);
            });
        }
    }
}
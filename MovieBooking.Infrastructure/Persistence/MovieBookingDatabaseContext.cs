using Microsoft.EntityFrameworkCore;
using MovieBooking.Domain.Entities;

namespace MovieBooking.Infrastructure.Persistence
{
    public class MovieBookingDatabaseContext : DbContext
    {
        public MovieBookingDatabaseContext(DbContextOptions<MovieBookingDatabaseContext> Options)
            : base(Options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder Builder)
        {
            Builder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();
        }
    }

}

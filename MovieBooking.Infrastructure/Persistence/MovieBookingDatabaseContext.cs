using Microsoft.EntityFrameworkCore;
using MovieBooking.Domain.Entities;
using System;

namespace MovieBooking.Infrastructure.Persistence
{
    public class MovieBookingDatabaseContext : DbContext
    {
        public MovieBookingDatabaseContext(DbContextOptions<MovieBookingDatabaseContext> Options)
            : base(Options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Screen> Screens => Set<Screen>();
        public DbSet<ShowTime> ShowTimes => Set<ShowTime>();

        public DbSet<Theatre> Theatres => Set<Theatre>();
        public DbSet<AdminRequest> AdminRequests => Set<AdminRequest>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<TheatreTimeSlot> TheatreTimeSlots => Set<TheatreTimeSlot>();

        public DbSet<Language> Languages => Set<Language>();
        public DbSet<Seat> Seats => Set<Seat>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.Name).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.Role).IsRequired();
                entity.Property(u => u.IsActive).IsRequired();

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
            modelBuilder.Entity<TheatreTimeSlot>(entity =>
            {
                entity.HasKey(t => t.TheatreTimeSlotId);

                entity.HasOne(t => t.Theatre)
                      .WithMany(th => th.TimeSlots)
                      .HasForeignKey(t => t.TheatreId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(t => new { t.TheatreId, t.StartTime })
                      .IsUnique();
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(m => m.MovieId);

                entity.Property(m => m.Title).IsRequired();
                entity.Property(m => m.DurationMinutes).IsRequired();
                entity.Property(m => m.ReleaseDate).IsRequired();
                entity.Property(m => m.IsActive).IsRequired();
                entity.Property(m => m.PosterUrl).HasMaxLength(500);

                entity.Property(m => m.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

           
            modelBuilder.Entity<Theatre>(entity =>
            {
                entity.HasKey(t => t.TheatreId);

                entity.Property(t => t.Name).IsRequired();
                entity.Property(t => t.Location).IsRequired();
                entity.Property(t => t.CreatedBy).IsRequired();
                entity.Property(t => t.ApprovalStatus).IsRequired();
                entity.Property(t => t.IsActive).IsRequired();

                entity.Property(t => t.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });


            modelBuilder.Entity<Screen>(entity =>
            {
                entity.HasKey(s => s.ScreenId);

                entity.Property(s => s.ScreenName)
                      .IsRequired();

                entity.Property(s => s.SeatLayoutType)
                      .IsRequired();

                entity.Property(s => s.IsActive)
                      .IsRequired();

                entity.HasOne<Theatre>()
                      .WithMany(t => t.Screens)
                      .HasForeignKey(s => s.TheatreId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasKey(l => l.LanguageId);
                entity.Property(l => l.Name).IsRequired();
                entity.HasIndex(l => l.Name).IsUnique();
            });

          
            modelBuilder.Entity<ShowTime>(entity =>
            {
                entity.HasKey(st => st.ShowTimeId);

                entity.Property(st => st.StartTime).IsRequired();
                entity.Property(st => st.EndTime).IsRequired();
                entity.Property(st => st.BasePrice).HasPrecision(10, 2);
                entity.Property(st => st.ApprovalStatus).IsRequired();
                entity.Property(st => st.IsActive).IsRequired();

                entity.HasOne(st => st.Movie)
                      .WithMany(m => m.ShowTimes)
                      .HasForeignKey(st => st.MovieId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(st => st.Theatre)
                      .WithMany()
                      .HasForeignKey(st => st.TheatreId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(st => st.Screen)
                      .WithMany(s => s.ShowTimes)
                      .HasForeignKey(st => st.ScreenId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(st => st.Language)
                      .WithMany(l => l.ShowTimes)
                      .HasForeignKey(st => st.LanguageId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            
            modelBuilder.Entity<AdminRequest>(entity =>
            {
                entity.HasKey(ar => ar.AdminRequestId);

                entity.Property(ar => ar.RequestType).IsRequired();
                entity.Property(ar => ar.ReferenceId).IsRequired();
                entity.Property(ar => ar.Status).IsRequired();

                entity.Property(ar => ar.RequestedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(s => s.SeatId);

                entity.Property(s => s.SeatRow).IsRequired();
                entity.Property(s => s.SeatColumn).IsRequired();
                entity.Property(s => s.PriceMultiplier).HasPrecision(5, 2);
                entity.Property(s => s.IsActive).IsRequired();

                entity.HasOne(s => s.Screen)
                      .WithMany(sc => sc.Seats)
                      .HasForeignKey(s => s.ScreenId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


        }

    }

}

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

        public DbSet<Language> Languages => Set<Language>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.Property(u => u.Name)
                      .IsRequired();

                entity.Property(u => u.Email)
                      .IsRequired();

                entity.Property(u => u.Password)
                      .IsRequired();

                entity.Property(u => u.Role)
                      .IsRequired();

                entity.Property(u => u.IsActive)
                      .IsRequired();

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

         
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(m => m.MovieId);

                entity.Property(m => m.Title)
                      .IsRequired();

                entity.Property(m => m.Description);

                entity.Property(m => m.DurationMinutes)
                      .IsRequired();

                entity.Property(m => m.ReleaseDate)
                      .IsRequired();

                entity.Property(m => m.IsActive)
                      .IsRequired();

                entity.Property(m => m.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            
            modelBuilder.Entity<Theatre>(entity =>
            {
                entity.HasKey(t => t.TheatreId);

                entity.Property(t => t.Name)
                      .IsRequired();

                entity.Property(t => t.Location)
                      .IsRequired();

                entity.Property(t => t.CreatedBy)
                      .IsRequired();

                entity.Property(t => t.ApprovalStatus)
                      .IsRequired();

                entity.Property(t => t.IsActive)
                      .IsRequired();

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
                      .WithMany()
                      .HasForeignKey(s => s.TheatreId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            
            modelBuilder.Entity<ShowTime>(entity =>
            {
                entity.HasKey(st => st.ShowTimeId);

                entity.Property(st => st.StartTime)
                      .IsRequired();

                entity.Property(st => st.EndTime)
                      .IsRequired();

                entity.Property(st => st.BasePrice)
                      .HasPrecision(10, 2); // FIXES YOUR WARNING

                entity.Property(st => st.ApprovalStatus)
                      .IsRequired();

                entity.Property(st => st.IsActive)
                      .IsRequired();

                entity.HasOne<Movie>()
                      .WithMany()
                      .HasForeignKey(st => st.MovieId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Theatre>()
                      .WithMany()
                      .HasForeignKey(st => st.TheatreId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Screen>()
                      .WithMany()
                      .HasForeignKey(st => st.ScreenId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            
            modelBuilder.Entity<AdminRequest>(entity =>
            {
                entity.HasKey(ar => ar.AdminRequestId);

                entity.Property(ar => ar.RequestType)
                      .IsRequired();

                entity.Property(ar => ar.ReferenceId)
                      .IsRequired();

                entity.Property(ar => ar.Status)
                      .IsRequired();

                entity.Property(ar => ar.RequestedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<ShowTime>()
                .HasOne(st => st.Language)
                .WithMany(l => l.ShowTimes)
                .HasForeignKey(st => st.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);

        }



    }

}

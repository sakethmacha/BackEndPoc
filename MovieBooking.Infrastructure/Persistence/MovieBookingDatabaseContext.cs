using Microsoft.EntityFrameworkCore;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;
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
        //
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<SeatLock> SeatLocks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }
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
            // Add this to your OnModelCreating method in MovieBookingDatabaseContext

            // ========== BOOKING ENTITY ==========
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.BookingId);

                entity.Property(b => b.TotalAmount)
                      .HasPrecision(10, 2)
                      .IsRequired();

                entity.Property(b => b.Status)
                      .IsRequired();

                entity.Property(b => b.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(b => b.BookingTime)
                      .IsRequired(false);

                entity.Property(b => b.PaymentId)
                      .IsRequired(false);

                // Relationships
                entity.HasOne(b => b.User)
                      .WithMany()
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.ShowTime)
                      .WithMany()
                      .HasForeignKey(b => b.ShowTimeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Payment)
                      .WithOne(p => p.Booking)
                      .HasForeignKey<Booking>(b => b.PaymentId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                // Indexes
                entity.HasIndex(b => b.UserId);
                entity.HasIndex(b => b.ShowTimeId);
                entity.HasIndex(b => b.Status);
                entity.HasIndex(b => b.CreatedAt);
            });

            // ========== BOOKING SEAT ENTITY ==========
            modelBuilder.Entity<BookingSeat>(entity =>
            {
                entity.HasKey(bs => bs.BookingSeatId);

                entity.Property(bs => bs.SeatPrice)
                      .HasPrecision(10, 2)
                      .IsRequired();

                entity.Property(bs => bs.Status)
                      .IsRequired();

                // Relationships
                entity.HasOne(bs => bs.Booking)
                      .WithMany(b => b.BookingSeats)
                      .HasForeignKey(bs => bs.BookingId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete when booking is deleted

                entity.HasOne(bs => bs.Seat)
                      .WithMany()
                      .HasForeignKey(bs => bs.SeatId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bs => bs.ShowTime)
                      .WithMany()
                      .HasForeignKey(bs => bs.ShowTimeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Composite unique index - prevent double booking same seat for same show
                entity.HasIndex(bs => new { bs.ShowTimeId, bs.SeatId })
                      .IsUnique()
                      .HasFilter("[Status] != 2"); // Don't apply unique constraint for cancelled bookings (Status = CANCELLED = 2)

                // Additional indexes for performance
                entity.HasIndex(bs => bs.BookingId);
                entity.HasIndex(bs => bs.ShowTimeId);
            });

            // ========== SEAT LOCK ENTITY ==========
            modelBuilder.Entity<SeatLock>(entity =>
            {
                entity.HasKey(sl => sl.SeatLockId);

                entity.Property(sl => sl.LockedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(sl => sl.ExpiresAt)
                      .IsRequired();

                entity.Property(sl => sl.Status)
                      .IsRequired();

                // Relationships
                entity.HasOne(sl => sl.ShowTime)
                      .WithMany()
                      .HasForeignKey(sl => sl.ShowTimeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sl => sl.Seat)
                      .WithMany()
                      .HasForeignKey(sl => sl.SeatId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sl => sl.User)
                      .WithMany()
                      .HasForeignKey(sl => sl.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // ✅ Composite UNIQUE filtered index (only ACTIVE locks)
                entity.HasIndex(sl => new { sl.ShowTimeId, sl.SeatId })
                      .IsUnique()
                      .HasFilter("[Status] = 0"); // ACTIVE / LOCKED only

                // Supporting indexes
                entity.HasIndex(sl => sl.ExpiresAt);
                entity.HasIndex(sl => sl.UserId);
                entity.HasIndex(sl => new { sl.ShowTimeId, sl.Status });
            });


            // ========== PAYMENT ENTITY ==========
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.PaymentId);

                entity.Property(p => p.Amount)
                      .HasPrecision(10, 2)
                      .IsRequired();

                entity.Property(p => p.PaymentMethod)
                      .IsRequired();

                entity.Property(p => p.PaymentStatus)
                      .IsRequired();

                entity.Property(p => p.TransactionId)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.PaymentGateway)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(p => p.PaidAt)
                      .IsRequired(false);

                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                // Relationship already defined in Booking entity (one-to-one)

                // Indexes
                entity.HasIndex(p => p.BookingId)
                      .IsUnique();

                entity.HasIndex(p => p.TransactionId)
                      .IsUnique();

                entity.HasIndex(p => p.PaymentStatus);
                entity.HasIndex(p => p.CreatedAt);
            });

            // ========== NOTIFICATION LOG ENTITY ==========
            modelBuilder.Entity<NotificationLog>(entity =>
            {
                entity.HasKey(nl => nl.NotificationLogId);

                entity.Property(nl => nl.Type)
                      .IsRequired();

                entity.Property(nl => nl.Message)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.Property(nl => nl.SentAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(nl => nl.Status)
                      .IsRequired();

                // Relationships
                entity.HasOne(nl => nl.User)
                      .WithMany()
                      .HasForeignKey(nl => nl.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(nl => nl.UserId);
                entity.HasIndex(nl => nl.SentAt);
                entity.HasIndex(nl => nl.Status);
            });
            // Add this to OnModelCreating method

            // AdminRequest Configuration
            modelBuilder.Entity<AdminRequest>(entity =>
            {
                entity.HasKey(e => e.AdminRequestId);

                entity.HasOne(e => e.RequestedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.RequestedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Screen Configuration - Add ApprovalStatus
            modelBuilder.Entity<Screen>(entity =>
            {
                entity.HasKey(e => e.ScreenId);

                entity.Property(e => e.ApprovalStatus)
                    .HasDefaultValue(ApprovalStatus.APPROVED);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Theatre)
                    .WithMany(t => t.Screens)
                    .HasForeignKey(e => e.TheatreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }

}

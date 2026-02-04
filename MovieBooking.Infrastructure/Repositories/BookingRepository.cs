
using Microsoft.EntityFrameworkCore;
using MovieBooking.Application.Interfaces.Repositories;
using MovieBooking.Domain.Entities;
using MovieBooking.Domain.Enums;
using MovieBooking.Infrastructure.Persistence;

namespace MovieBooking.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly MovieBookingDatabaseContext DbContext;

        public BookingRepository(MovieBookingDatabaseContext context)
        {
            DbContext = context;
        }

        // ========== MOVIE & SHOWTIME QUERIES ==========

        public async Task<List<Movie>> GetActiveMoviesAsync()
        {
            return await DbContext.Movies
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
        }

        public async Task<List<ShowTime>> GetShowTimesByMovieAsync(Guid movieId, DateOnly date)
        {
            var startOfDay = date.ToDateTime(TimeOnly.MinValue);
            var endOfDay = date.ToDateTime(TimeOnly.MaxValue);

            return await DbContext.ShowTimes
                .Include(st => st.Theatre)
                .Include(st => st.Screen)
                .Include(st => st.Language)
                .Include(st => st.Movie)
                .Where(st => st.MovieId == movieId
                    && st.IsActive
                    && st.StartTime >= startOfDay
                    && st.StartTime <= endOfDay)
                .OrderBy(st => st.StartTime)
                .ToListAsync();
        }

        public async Task<ShowTime> GetShowTimeByIdAsync(Guid showTimeId)
        {
            var showTime = await DbContext.ShowTimes
                .Include(st => st.Movie)
                .Include(st => st.Theatre)
                .Include(st => st.Screen)
                .Include(st => st.Language)
                .FirstOrDefaultAsync(st => st.ShowTimeId == showTimeId);

            if (showTime == null)
                throw new InvalidOperationException("ShowTime not found");

            return showTime;
        }

        // ========== SEAT QUERIES ==========

        public async Task<List<Seat>> GetSeatsByScreenAsync(Guid screenId)
        {
            return await DbContext.Seats
                .Where(s => s.ScreenId == screenId && s.IsActive)
                .OrderBy(s => s.SeatRow)
                .ThenBy(s => s.SeatColumn)
                .ToListAsync();
        }

        public async Task<List<BookingSeat>> GetBookedSeatsForShowAsync(Guid showTimeId)
        {
            return await DbContext.BookingSeats
                .Include(bs => bs.Booking)
                .Include(bs => bs.Seat)
                .Where(bs => bs.ShowTimeId == showTimeId
                    && (bs.Booking.Status == BookingStatus.CONFIRMED
                        || bs.Booking.Status == BookingStatus.PENDING))
                .ToListAsync();
        }

        public async Task<List<SeatLock>> GetActiveSeatLocksForShowAsync(Guid showTimeId)
        {
            var now = DateTime.UtcNow;
            return await DbContext.SeatLocks
                .Where(sl => sl.ShowTimeId == showTimeId
                    && sl.Status == SeatLockStatus.LOCKED
                    && sl.ExpiresAt > now)
                .ToListAsync();
        }

        // ========== SEAT LOCKING ==========

        public async Task<List<SeatLock>> LockSeatsAsync(Guid userId, Guid showTimeId, List<Guid> seatIds)
        {
            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(5);

            var locks = seatIds.Select(seatId => new SeatLock
            {
                SeatLockId = Guid.NewGuid(),
                ShowTimeId = showTimeId,
                SeatId = seatId,
                UserId = userId,
                LockedAt = now,
                ExpiresAt = expiresAt,
                Status = SeatLockStatus.LOCKED
            }).ToList();

            DbContext.SeatLocks.AddRange(locks);
            await DbContext.SaveChangesAsync();

            return locks;
        }

        public async Task ReleaseSeatLocksAsync(List<Guid> seatLockIds)
        {
            var locks = await DbContext.SeatLocks
                .Where(sl => seatLockIds.Contains(sl.SeatLockId))
                .ToListAsync();

            foreach (var seatlock in locks)
            {
                seatlock.Status = SeatLockStatus.EXPIRED;
            }
            var now = DateTime.UtcNow;

            var expiredBookings = await DbContext.Bookings
       .Where(b =>
           b.Status == BookingStatus.PENDING &&
           b.CreatedAt.AddMinutes(5) <= now)
       .ToListAsync();

            foreach (var booking in expiredBookings)
            {
                booking.Status = BookingStatus.FAILED;
            }
            await DbContext.SaveChangesAsync();
        }

        public async Task ReleaseExpiredLocksAsync()
        {
            var now = DateTime.UtcNow;
            var expiredLocks = await DbContext.SeatLocks
                .Where(sl => sl.Status == SeatLockStatus.LOCKED && sl.ExpiresAt <= now)
                .ToListAsync();

            foreach (var seatlock in expiredLocks)
            {
                seatlock.Status = SeatLockStatus.EXPIRED;
            }

            await DbContext.SaveChangesAsync();
        }

        public async Task<bool> AreSeatsAvailableAsync(Guid showTimeId, List<Guid> seatIds)
        {
            var now = DateTime.UtcNow;

            // Check if seats are already booked
            var bookedSeats = await DbContext.BookingSeats
                .Where(bs => bs.ShowTimeId == showTimeId
                    && seatIds.Contains(bs.SeatId)
                    && (bs.Booking.Status == BookingStatus.CONFIRMED
                        || bs.Booking.Status == BookingStatus.PENDING))
                .Select(bs => bs.SeatId)
                .ToListAsync();

            if (bookedSeats.Any())
                return false;

            // Check if seats are locked by other users
            var lockedSeats = await DbContext.SeatLocks
                .Where(sl => sl.ShowTimeId == showTimeId
                    && seatIds.Contains(sl.SeatId)
                    && sl.Status == SeatLockStatus.LOCKED
                    && sl.ExpiresAt > now)
                .Select(sl => sl.SeatId)
                .ToListAsync();

            return !lockedSeats.Any();
        }

        // ========== BOOKING OPERATIONS ==========

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            DbContext.Bookings.Add(booking);
            await DbContext.SaveChangesAsync();
            return booking;
        }

        public async Task<List<BookingSeat>> CreateBookingSeatsAsync(List<BookingSeat> bookingSeats)
        {
            DbContext.BookingSeats.AddRange(bookingSeats);
            await DbContext.SaveChangesAsync();
            return bookingSeats;
        }

        public async Task<Booking> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await DbContext.Bookings
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Movie)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Theatre)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Screen)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Language)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                throw new InvalidOperationException("Booking not found");

            return booking;
        }

        public async Task<List<Booking>> GetUserBookingsAsync(Guid userId)
        {
            return await DbContext.Bookings
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Movie)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Theatre)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Screen)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            DbContext.Bookings.Update(booking);
            await DbContext.SaveChangesAsync();
        }

        // ========== PAYMENT OPERATIONS ==========

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            DbContext.Payments.Add(payment);
            await DbContext.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> GetPaymentByIdAsync(Guid paymentId)
        {
            var payment = await DbContext.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                throw new InvalidOperationException("Payment not found");

            return payment;
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            DbContext.Payments.Update(payment);
            await DbContext.SaveChangesAsync();
        }

        // ========== NOTIFICATION ==========

        public async Task AddNotificationLogAsync(NotificationLog log)
        {
            DbContext.NotificationLogs.Add(log);
            await DbContext.SaveChangesAsync();
        }

        // ========== USER ==========

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await DbContext.Users.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");
            return user;
        }
        //
       
        public async Task<List<BookingSeat>> GetLockedBookingSeatsAsync(
    Guid showTimeId,
    List<Guid> seatIds)
        {
            return await DbContext.BookingSeats
                .Where(bs =>
                    bs.ShowTimeId == showTimeId &&
                    seatIds.Contains(bs.SeatId) &&
                    bs.Status == SeatLockStatus.LOCKED)
                .ToListAsync();
        }
        public async Task<bool> CanUserBookSeatsAsync(Guid showTimeId,List<Guid> seatIds,Guid userId)
        {
            var now = DateTime.UtcNow;

            return !await DbContext.SeatLocks
                .AnyAsync(sl =>
                    sl.ShowTimeId == showTimeId
                    && seatIds.Contains(sl.SeatId)
                    &&
                    (
                        // ❌ Seat already booked (final)
                        sl.Status == SeatLockStatus.BOOKED

                        // ❌ Seat locked by another user and not expired
                        || (sl.Status == SeatLockStatus.LOCKED
                            && sl.UserId != userId
                            && sl.ExpiresAt > now)
                    )
                );
        }
        // MovieBooking.Infrastructure/Repositories/BookingRepository.cs

        public async Task ConvertLocksToBookingAsync(Guid userId, Guid showTimeId, List<Guid> seatIds)
        {
            var locks = await DbContext.SeatLocks
                .Where(sl => sl.UserId == userId
                    && sl.ShowTimeId == showTimeId
                    && seatIds.Contains(sl.SeatId)
                    && sl.Status == SeatLockStatus.LOCKED)
                .ToListAsync();

            // Don't delete locks, just mark them as expired since booking is created
            foreach (var seatlock in locks)
    {
                seatlock.Status = SeatLockStatus.EXPIRED;
            }

            await DbContext.SaveChangesAsync();
        }

    }
}
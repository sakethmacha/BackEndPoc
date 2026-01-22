
using MovieBooking.Domain.Entities;

namespace MovieBooking.Application.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        // Movie & ShowTime Queries
        Task<List<Movie>> GetActiveMoviesAsync();
        Task<List<ShowTime>> GetShowTimesByMovieAsync(Guid movieId, DateOnly date);
        Task<ShowTime> GetShowTimeByIdAsync(Guid showTimeId);

        // Seat Queries
        Task<List<Seat>> GetSeatsByScreenAsync(Guid screenId);
        Task<List<BookingSeat>> GetBookedSeatsForShowAsync(Guid showTimeId);
        Task<List<SeatLock>> GetActiveSeatLocksForShowAsync(Guid showTimeId);

        // Seat Locking
        Task<List<SeatLock>> LockSeatsAsync(Guid userId, Guid showTimeId, List<Guid> seatIds);
        Task ReleaseSeatLocksAsync(List<Guid> seatLockIds);
        Task ReleaseExpiredLocksAsync();
        Task<bool> AreSeatsAvailableAsync(Guid showTimeId, List<Guid> seatIds);

        // Booking Operations
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<List<BookingSeat>> CreateBookingSeatsAsync(List<BookingSeat> bookingSeats);
        Task<Booking> GetBookingByIdAsync(Guid bookingId);
        Task<List<Booking>> GetUserBookingsAsync(Guid userId);
        Task UpdateBookingAsync(Booking booking);

        // Payment Operations
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> GetPaymentByIdAsync(Guid paymentId);
        Task UpdatePaymentAsync(Payment payment);

        // Notification
        Task AddNotificationLogAsync(NotificationLog log);

        // User
        Task<User> GetUserByIdAsync(Guid userId);
    }
}
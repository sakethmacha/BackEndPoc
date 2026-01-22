// MovieBooking.Application/Interfaces/Services/IBookingService.cs
using MovieBooking.Application.DTOs.Booking;

namespace MovieBooking.Application.Interfaces.Services
{
    public interface IBookingService
    {
        // Browse Movies & Shows
        Task<List<MovieListDto>> GetActiveMoviesAsync();
        Task<List<TheatreShowDto>> GetShowTimesByMovieAsync(Guid movieId, DateOnly date);

        // Seat Selection
        Task<SeatLayoutDto> GetSeatLayoutAsync(Guid showTimeId);
        Task<LockSeatsResponseDto> LockSeatsAsync(Guid userId, LockSeatsRequestDto request);

        // Booking & Payment
        Task<BookingConfirmationDto> CreateBookingAsync(Guid userId, CreateBookingRequestDto request);
        Task<PaymentResponseDto> ProcessPaymentAsync(Guid userId, ProcessPaymentRequestDto request);

        // User Bookings
        Task<List<UserBookingDto>> GetUserBookingsAsync(Guid userId);
        Task<BookingConfirmationDto> GetBookingDetailsAsync(Guid userId, Guid bookingId);
        Task CancelBookingAsync(Guid userId, CancelBookingRequestDto request);

        // Background Tasks
        Task ReleaseExpiredSeatLocksAsync();
    }
}
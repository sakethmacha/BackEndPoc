using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.Application.DTOs.Booking;
using MovieBooking.Application.Interfaces.Services;
using System.Security.Claims;

namespace MovieBooking.Api.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize(Roles = "User")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // ========== BROWSE MOVIES & SHOWS ==========

        /// <summary>
        /// Get all active movies
        /// </summary>
        [HttpGet("movies")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveMovies()
        {
            var movies = await _bookingService.GetActiveMoviesAsync();
            return Ok(movies);
        }

        /// <summary>
        /// Get showtimes for a specific movie on a date
        /// </summary>
        [HttpGet("movies/{movieId}/showtimes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShowTimes(Guid movieId, [FromQuery] string date)
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
                return BadRequest("Invalid date format. Use YYYY-MM-DD");

            var showTimes = await _bookingService.GetShowTimesByMovieAsync(movieId, parsedDate);
            return Ok(showTimes);
        }

        // ========== SEAT SELECTION ==========

        /// <summary>
        /// Get seat layout for a showtime
        /// </summary>
        [HttpGet("showtimes/{showTimeId}/seats")]
        public async Task<IActionResult> GetSeatLayout(Guid showTimeId)
        {
            var seatLayout = await _bookingService.GetSeatLayoutAsync(showTimeId);
            return Ok(seatLayout);
        }

        /// <summary>
        /// Lock selected seats temporarily (5 minutes)
        /// </summary>
        [HttpPost("lock-seats")]
        public async Task<IActionResult> LockSeats([FromBody] LockSeatsRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _bookingService.LockSeatsAsync(userId, request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // ========== BOOKING & PAYMENT ==========

        /// <summary>
        /// Create a new booking
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var booking = await _bookingService.CreateBookingAsync(userId, request);
            return Ok(booking);
        }

        /// <summary>
        /// Process payment for a booking
        /// </summary>
        [HttpPost("payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            var payment = await _bookingService.ProcessPaymentAsync(userId, request);
            return Ok(payment);
        }

        // ========== USER BOOKINGS ==========

        /// <summary>
        /// Get all bookings for the current user
        /// </summary>
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(bookings);
        }

        /// <summary>
        /// Get details of a specific booking
        /// </summary>
        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingDetails(Guid bookingId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var booking = await _bookingService.GetBookingDetailsAsync(userId, bookingId);
            return Ok(booking);
        }

        /// <summary>
        /// Cancel a booking
        /// </summary>
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _bookingService.CancelBookingAsync(userId, request);
            return Ok(new { message = "Booking cancelled successfully" });
        }
    }
}
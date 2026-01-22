
namespace MovieBooking.Application.DTOs.Booking
{
    public class BookedSeatDto
    {
        public string SeatNumber { get; set; } = string.Empty;
        public string SeatType { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
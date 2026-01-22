
namespace MovieBooking.Application.DTOs.Booking
{
    public class SeatDto
    {
        public Guid SeatId { get; set; }
        public string SeatRow { get; set; } = string.Empty;
        public int SeatColumn { get; set; }
        public string SeatNumber { get; set; } = string.Empty; // e.g., "A1"
        public string SeatType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsLocked { get; set; }
    }
}
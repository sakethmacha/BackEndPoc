// MovieBooking.Application/DTOs/Booking/LockedSeatDto.cs
namespace MovieBooking.Application.DTOs.Booking
{
    public class LockedSeatDto
    {
        public Guid SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
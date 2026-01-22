
namespace MovieBooking.Application.DTOs.Booking
{
    public class LockSeatsRequestDto
    {
        public Guid ShowTimeId { get; set; }
        public List<Guid> SeatIds { get; set; } = new();
    }
}
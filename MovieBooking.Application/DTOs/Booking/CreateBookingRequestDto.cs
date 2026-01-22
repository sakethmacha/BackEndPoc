// MovieBooking.Application/DTOs/Booking/CreateBookingRequestDto.cs
namespace MovieBooking.Application.DTOs.Booking
{
    public class CreateBookingRequestDto
    {
        public Guid ShowTimeId { get; set; }
        public List<Guid> SeatIds { get; set; } = new();
    }
}
// MovieBooking.Application/DTOs/Booking/CancelBookingRequestDto.cs
namespace MovieBooking.Application.DTOs.Booking
{
    public class CancelBookingRequestDto
    {
        public Guid BookingId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
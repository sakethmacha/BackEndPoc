
namespace MovieBooking.Application.DTOs.Booking
{
    public class BookingConfirmationDto
    {
        public Guid BookingId { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public MovieDetailsDto Movie { get; set; } = null!;
        public TheatreDetailsDto Theatre { get; set; } = null!;
        public List<BookedSeatDto> Seats { get; set; } = new();
    }
}
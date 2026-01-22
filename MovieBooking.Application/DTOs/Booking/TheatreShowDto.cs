
namespace MovieBooking.Application.DTOs.Booking
{
    public class TheatreShowDto
    {
        public Guid TheatreId { get; set; }
        public string TheatreName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<ShowDto> Shows { get; set; } = new();
    }
}
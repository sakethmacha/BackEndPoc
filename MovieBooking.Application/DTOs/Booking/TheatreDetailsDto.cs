
namespace MovieBooking.Application.DTOs.Booking
{
    public class TheatreDetailsDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public DateTime ShowTime { get; set; }
    }
}

namespace MovieBooking.Application.DTOs.Booking
{
    public class ShowDto
    {
        public Guid ShowTimeId { get; set; }
        public Guid ScreenId { get; set; }
        public string ScreenName { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal BasePrice { get; set; }
        public int AvailableSeats { get; set; }
    }
}
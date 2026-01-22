// MovieBooking.Application/DTOs/Booking/SeatLayoutDto.cs
namespace MovieBooking.Application.DTOs.Booking
{
    public class SeatLayoutDto
    {
        public Guid ShowTimeId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string TheatreName { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public decimal BasePrice { get; set; }
        public List<SeatRowDto> SeatRows { get; set; } = new();
    }
}
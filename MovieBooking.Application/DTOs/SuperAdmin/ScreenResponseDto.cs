using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class ScreenResponseDto
    {
        public Guid ScreenId { get; set; }
        public Guid TheatreId { get; set; }
        public string ScreenName { get; set; } = string.Empty;
        public SeatLayoutType SeatLayoutType { get; set; }
    }
}

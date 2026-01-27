using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.DTOs.Admin
{
    public class CreateScreenRequestDto
    {
        public Guid TheatreId { get; set; }
        public string ScreenName { get; set; }
        public string SeatLayoutType { get; set; }
        public List<CreateSeatRowDto> SeatRows { get; set; } = new();
    }
}

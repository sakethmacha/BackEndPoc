using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class CreateScreenDto
    {
        public Guid TheatreId { get; set; }
        public string ScreenName { get; set; }
        public SeatLayoutType SeatLayoutType { get; set; }

        public bool IsActive { get; set; } = true;
        public List<CreateSeatRowDto> SeatRows { get; set; } = new();
    }
}

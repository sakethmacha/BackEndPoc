using MovieBooking.Application.DTOs.SuperAdmin;

namespace MovieBooking.Application.DTOs.Admin
{
    public class CreateTheatreRequestDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public List<TimeSlotResponseDto> TimeSlots { get; set; } = new();
    }
}

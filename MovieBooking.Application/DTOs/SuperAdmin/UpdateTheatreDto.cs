namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class UpdateTheatreDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public List<TimeSlotDto> TimeSlots { get; set; } = new();
    }
}

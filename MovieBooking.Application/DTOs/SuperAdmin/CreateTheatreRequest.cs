namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class CreateTheatreRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public List<TimeSlotDto> TimeSlots { get; set; } = new();
    }

}

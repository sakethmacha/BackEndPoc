namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class CreateTheatreDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public Guid SuperAdminId { get; set; } // from token ideally

        public List<TimeSlotDto> TimeSlots { get; set; } = new();
    }
}

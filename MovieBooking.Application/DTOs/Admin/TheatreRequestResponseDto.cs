using MovieBooking.Domain.Enums;
namespace MovieBooking.Application.DTOs.Admin
{
    public class TheatreRequestResponseDto
    {
        public Guid TheatreId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime RequestedAt { get; set; }
        public List<TimeSlotResponseDto> TimeSlots { get; set; } = new();
    }
}

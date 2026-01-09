using MovieBooking.Domain.Enums;

namespace MovieBooking.Domain.Entities
{
    public class Theatre
    {
        public Guid TheatreId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public Guid CreatedBy { get; set; }   // Admin

        public ApprovalStatus ApprovalStatus { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Screen> Screens { get; set; } = new List<Screen>();
    }

}

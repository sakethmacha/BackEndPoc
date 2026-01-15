using MovieBooking.Domain.Enums;

namespace MovieBooking.Domain.Entities
{
    public class ShowTime
    {
        public Guid ShowTimeId { get; set; }

        public Guid TheatreId { get; set; }
        public Guid ScreenId { get; set; }
        public Guid MovieId { get; set; }
        public Guid LanguageId { get; set; }

        // Derived from ShowDate + TheatreTimeSlot
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; } = true;

        public Theatre Theatre { get; set; } = null!;
        public Screen Screen { get; set; } = null!;
        public Movie Movie { get; set; } = null!;
        public Language Language { get; set; } = null!;
    }


}

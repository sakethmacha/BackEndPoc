using MovieBooking.Domain.Enums;

namespace MovieBooking.Domain.Entities
{
    public class ShowTime
    {
        public Guid ShowTimeId { get; set; }

        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }
        public Guid TheatreId { get; set; }
        public Theatre Theatre { get; set; }
        public Guid ScreenId { get; set; }
        public Screen Screen { get; set; }
        public Guid LanguageId { get; set; }
        public Language Language { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal BasePrice { get; set; }

        public ApprovalStatus ApprovalStatus { get; set; }
        public bool IsActive { get; set; }
    }

}

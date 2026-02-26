using MovieBooking.Domain.Enums;

namespace MovieBooking.Domain.Entities
{
    public class Screen
    {
        public Guid ScreenId { get; set; }
        public Guid TheatreId { get; set; }

        public string ScreenName { get; set; }
        public SeatLayoutType SeatLayoutType { get; set; }

        public bool IsActive { get; set; } 

        public ApprovalStatus ApprovalStatus { get; set; }// ADD THIS
        public DateTime CreatedAt { get; set; } // ADD THIS

        // Navigation properties
        public ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public Theatre Theatre { get; set; } // ADD THIS if not present
    }

}

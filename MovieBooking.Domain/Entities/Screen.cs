using MovieBooking.Domain.Enums;

namespace MovieBooking.Domain.Entities
{
    public class Screen
    {
        public Guid ScreenId { get; set; }
        public Guid TheatreId { get; set; }

        public string ScreenName { get; set; }
        public SeatLayoutType SeatLayoutType { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();

    }

}

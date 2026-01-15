namespace MovieBooking.Domain.Entities
{
    public class TheatreTimeSlot
    {
        public Guid TheatreTimeSlotId { get; set; }
        public Guid TheatreId { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public bool IsActive { get; set; } = true;

        public Theatre Theatre { get; set; } = null!;
    }
}

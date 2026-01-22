
using MovieBooking.Domain.Enums;

namespace MovieBooking.Domain.Entities
{
    public class SeatLock
    {
        public Guid SeatLockId { get; set; }
        public Guid ShowTimeId { get; set; }
        public Guid SeatId { get; set; }
        public Guid UserId { get; set; }
        public DateTime LockedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public SeatLockStatus Status { get; set; }

        // Navigation Properties
        public ShowTime ShowTime { get; set; } = null!;
        public Seat Seat { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
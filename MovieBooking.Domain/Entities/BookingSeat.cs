
using MovieBooking.Domain.Enums;

namespace MovieBooking.Domain.Entities
{
    public class BookingSeat
    {
        public Guid BookingSeatId { get; set; }
        public Guid BookingId { get; set; }
        public Guid SeatId { get; set; }
        public Guid ShowTimeId { get; set; }
        public decimal SeatPrice { get; set; }
        public SeatLockStatus Status { get; set; }

        // Navigation Properties
        public Booking Booking { get; set; } = null!;
        public Seat Seat { get; set; } = null!;
        public ShowTime ShowTime { get; set; } = null!;
    }
}
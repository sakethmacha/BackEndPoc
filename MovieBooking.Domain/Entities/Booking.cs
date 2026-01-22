
using MovieBooking.Domain.Enums;

namespace MovieBooking.Domain.Entities
{
    public class Booking
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public Guid ShowTimeId { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }
        public Guid? PaymentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? BookingTime { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
        public ShowTime ShowTime { get; set; } = null!;
        public Payment? Payment { get; set; }
        public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    }
}
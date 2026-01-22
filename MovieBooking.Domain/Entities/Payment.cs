
using MovieBooking.Domain.Enums;

namespace MovieBooking.Domain.Entities
{
    public class Payment
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentGateway { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Property
        public Booking Booking { get; set; } = null!;
    }
}
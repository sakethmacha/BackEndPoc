// MovieBooking.Application/DTOs/Booking/ProcessPaymentRequestDto.cs
using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.DTOs.Booking
{
    public class ProcessPaymentRequestDto
    {
        public Guid BookingId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentGateway { get; set; } = "Razorpay"; // Default
    }
}
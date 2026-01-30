using FastEndpoints;
using FluentValidation;
using MovieBooking.Application.DTOs.Booking;
namespace MovieBooking.Api.FastEndPoints
{
    public class CancelBookingValidator : Validator<CancelBookingRequestDto>
    {
        public CancelBookingValidator()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty()
                .WithMessage("BookingId is required");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .MinimumLength(5)
                .WithMessage("Reason must be at least 5 characters");
        }
    }
}

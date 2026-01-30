using FastEndpoints;
using MovieBooking.Application.DTOs.Booking;

namespace MovieBooking.Api.FastEndPoints
{
    public class CancelBookingPreProcessor
        : PreProcessor<CancelBookingRequestDto, CancelBookingState>
    {
        public override Task PreProcessAsync(
            IPreProcessorContext<CancelBookingRequestDto> context,
            CancelBookingState state,
            CancellationToken ct)
        {
            var httpContext = context.HttpContext;
            var request = context.Request;

            if (!httpContext.User.Identity!.IsAuthenticated)
                throw new UnauthorizedAccessException();

            Console.WriteLine($"Cancel request for booking {request.BookingId}");

            return Task.CompletedTask;
        }
    }
}

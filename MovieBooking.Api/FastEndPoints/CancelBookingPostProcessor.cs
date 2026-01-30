using FastEndpoints;
using MovieBooking.Application.DTOs.Booking;

namespace MovieBooking.Api.FastEndPoints
{
    public class CancelBookingPostProcessor
        : PostProcessor<CancelBookingRequestDto, CancelBookingState, CancelBookingResponseDto>
    {
        public override Task PostProcessAsync(
            IPostProcessorContext<CancelBookingRequestDto, CancelBookingResponseDto> context,
            CancelBookingState state,
            CancellationToken ct)
        {
            var request = context.Request;

            Console.WriteLine($"Booking {request.BookingId} cancelled successfully");

            return Task.CompletedTask;
        }
    }
}

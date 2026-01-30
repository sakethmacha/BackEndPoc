using FastEndpoints;
using MovieBooking.Application.DTOs.Booking;
using MovieBooking.Application.Interfaces.Services;
using System.Security.Claims;

namespace MovieBooking.Api.FastEndPoints
{
    public class CancelBookingEndpoint : Endpoint<CancelBookingRequestDto, CancelBookingResponseDto>
    {
        private readonly IBookingService _bookingService;

        public CancelBookingEndpoint(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public override void Configure()
        {
            Post("/api/book/cancel");
            Roles("User");
            PreProcessor<CancelBookingPreProcessor>();
            PostProcessor<CancelBookingPostProcessor>();
            Summary(s =>
            {
                s.Summary = "Cancel a booking";
                s.Description = "Cancels a confirmed booking and processes refund";
                s.Response(204, "Booking cancelled successfully");
                s.Response(401, "Unauthorized");
                s.Response(400, "Cancellation not allowed");
            });
        }

        public override async Task HandleAsync(
            CancelBookingRequestDto request,
            CancellationToken ct)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _bookingService.CancelBookingAsync(userId, request);

            //  custom status code
            await SendAsync(
                        new CancelBookingResponseDto
                        {
                            Message = "Booking cancelled successfully"
                        },
                        StatusCodes.Status200OK,
                        ct);
            //HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            //Response = new EmptyResponse();
            //await SendNoContentAsync(ct);

        }
    }
}

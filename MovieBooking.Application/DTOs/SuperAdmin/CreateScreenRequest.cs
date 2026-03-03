using MovieBooking.Domain.Enums;

namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class CreateScreenRequest
    {
        public Guid TheatreId { get; set; }
        public string ScreenName { get; set; } = string.Empty;

        // string because it comes from client (MVC / Swagger)
        public SeatLayoutType SeatLayoutType { get; set; } 

        public List<CreateSeatRowRequest> SeatRows { get; set; } = new();
    }
}

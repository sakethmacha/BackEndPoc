namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class CreateScreenRequest
    {
        public Guid TheatreId { get; set; }
        public string ScreenName { get; set; } = string.Empty;

        // string because it comes from client (MVC / Swagger)
        public string SeatLayoutType { get; set; } = string.Empty;

        public List<CreateSeatRowRequest> SeatRows { get; set; } = new();
    }
}

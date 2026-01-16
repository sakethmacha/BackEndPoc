namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class UpdateScreenDto
    {
        public string ScreenName { get; set; }
        public string SeatLayoutType { get; set; }
        public List<CreateSeatRowRequest> SeatRows { get; set; } = new();
    }
}

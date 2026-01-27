namespace MovieBooking.Application.DTOs.Admin
{
    public class ScreenRequestResponseDto
    {
        public Guid ScreenId { get; set; }
        public string ScreenName { get; set; }
        public string TheatreName { get; set; }
        public string SeatLayoutType { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}

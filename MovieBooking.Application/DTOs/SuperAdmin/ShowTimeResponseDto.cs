namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class ShowTimeResponseDto
    {
        public Guid ShowTimeId { get; set; }

        public string MovieTitle { get; set; } = string.Empty;
        public string TheatreName { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal BasePrice { get; set; }
    }


}

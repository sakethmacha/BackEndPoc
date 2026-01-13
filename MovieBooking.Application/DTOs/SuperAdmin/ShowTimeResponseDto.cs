namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class ShowTimeResponseDto
    {
        public Guid ShowTimeId { get; set; }
        public Guid MovieId { get; set; }
        public Guid TheatreId { get; set; }
        public Guid ScreenId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal BasePrice { get; set; }
    }

}

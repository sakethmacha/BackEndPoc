
namespace MovieBooking.Application.DTOs.Booking
{
    public class MovieListDto
    {
        public Guid MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string PosterUrl { get; set; } = string.Empty;
    }
}
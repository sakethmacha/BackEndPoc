namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class AddMovieDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string PosterUrl { get; set; }
    }
}

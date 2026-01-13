namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class MovieResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public int DurationMinutes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool IsActive { get; set; }
    }

}

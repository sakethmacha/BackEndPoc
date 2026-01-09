namespace MovieBooking.Domain.Entities
{
    public class Movie
    {
        public Guid MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string PosterUrl { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
    }

}

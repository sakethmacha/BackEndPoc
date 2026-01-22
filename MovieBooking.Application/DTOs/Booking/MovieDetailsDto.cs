// MovieBooking.Application/DTOs/Booking/MovieDetailsDto.cs
namespace MovieBooking.Application.DTOs.Booking
{
    public class MovieDetailsDto
    {
        public string Title { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
    }
}
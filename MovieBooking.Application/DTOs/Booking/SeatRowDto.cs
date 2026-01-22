// MovieBooking.Application/DTOs/Booking/SeatRowDto.cs
namespace MovieBooking.Application.DTOs.Booking
{
    public class SeatRowDto
    {
        public string RowName { get; set; } = string.Empty;
        public List<SeatDto> Seats { get; set; } = new();
    }
}
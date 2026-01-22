// MovieBooking.Application/DTOs/Booking/LockSeatsResponseDto.cs
namespace MovieBooking.Application.DTOs.Booking
{
    public class LockSeatsResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
        public List<LockedSeatDto> LockedSeats { get; set; } = new();
    }
}
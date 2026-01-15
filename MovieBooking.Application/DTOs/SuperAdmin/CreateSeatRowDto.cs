using MovieBooking.Domain.Enums;
namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class CreateSeatRowDto
    {
        public string SeatRow { get; set; } = string.Empty; // A, B, C
        public int SeatCount { get; set; }                  // 10, 8
        public SeatType SeatType { get; set; }
        public decimal PriceMultiplier { get; set; }
    }
}

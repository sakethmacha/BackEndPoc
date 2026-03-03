using MovieBooking.Domain.Enums;
namespace MovieBooking.Domain.Entities
{
    public class Seat
    {
        public Guid SeatId { get; set; }

        public Guid ScreenId { get; set; }
     
        public string SeatRow { get; set; } = string.Empty;   // A, B, C
        public int SeatColumn { get; set; }                   // 1,2,3

        public SeatType SeatType { get; set; }                // NORMAL/VIP
        public decimal PriceMultiplier { get; set; }

        public bool IsActive { get; set; } = true;

        public Screen Screen { get; set; } = null!;

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class CreateSeatRowRequest
    {
        public string SeatRow { get; set; } = string.Empty;   // A, B, C
        public int SeatCount { get; set; }                    // 10, 8
        public string SeatType { get; set; } = string.Empty;  // NORMAL / VIP
        public decimal PriceMultiplier { get; set; }          // 1.0 / 1.5
    }
}

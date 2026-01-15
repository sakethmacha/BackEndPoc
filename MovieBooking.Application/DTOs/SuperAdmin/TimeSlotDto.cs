using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class TimeSlotDto
    {
        public string StartTime { get; set; } = string.Empty; // "06:30"
        public string EndTime { get; set; } = string.Empty;   // "09:30"
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class UpdateShowTimeDto
    {
        public Guid MovieId { get; set; }
        public Guid LanguageId { get; set; }
        public DateOnly ShowDate { get; set; }
        public decimal BasePrice { get; set; }
    }
}

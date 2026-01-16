using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class UpdateLanguageDto
    {
        public Guid LanguageId { get; set; }
        public string Name { get; set; }
    }
}

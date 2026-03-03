using MovieBooking.Domain.Enums;
namespace MovieBooking.Application.DTOs.SuperAdmin
{
    public class AdminRequestResponseDto
    {
        public Guid AdminRequestId { get; set; }
        public string RequestType { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }

        public string RequestedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string RequestDetails { get; set; }

        //public string TheatreName { get; set; }
        //public string ScreenName { get; set; }
    }

}

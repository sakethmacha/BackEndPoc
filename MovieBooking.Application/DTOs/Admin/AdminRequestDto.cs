using MovieBooking.Domain.Enums;
namespace MovieBooking.Application.DTOs.Admin
{
    public class AdminRequestDto
    {
        public Guid AdminRequestId { get; set; }
        public string RequestType { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string RequestDetails { get; set; }

    }
}

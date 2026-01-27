using MovieBooking.Domain.Enums;

namespace MovieBooking.Domain.Entities
{
    public class AdminRequest
    {
        public Guid AdminRequestId { get; set; }
        public Guid RequestedBy { get; set; } // ADD THIS
        public User RequestedByUser { get; set; } // ADD THIS (Navigation property)
        public RequestType RequestType { get; set; }
        public Guid ReferenceId { get; set; }
        public ApprovalStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public Guid? ReviewedBy { get; set; }
    }
}
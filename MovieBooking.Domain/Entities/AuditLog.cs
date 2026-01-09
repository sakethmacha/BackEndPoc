namespace MovieBooking.Domain.Entities
{
    public class AuditLog
    {
        public Guid AuditLogId { get; set; }
        public Guid UserId { get; set; }

        public string Action { get; set; }
        public string EntityName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}

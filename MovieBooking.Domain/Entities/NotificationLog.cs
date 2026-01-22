
using MovieBooking.Domain.Enums;

namespace MovieBooking.Domain.Entities
{
    public class NotificationLog
    {
        public Guid NotificationLogId { get; set; }
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public NotificationStatus Status { get; set; }

        // Navigation Property
        public User User { get; set; } = null!;
    }
}
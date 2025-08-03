namespace eshop.Domain.Entities
{
    public class UserNotification : IBaseEntity
    {
        public Guid Id { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime ReadAt { get; set; }

        // Navigational properties
        public Guid NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}

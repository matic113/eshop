namespace eshop.Domain.Entities
{
    public class Notification : IBaseEntity
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // if userId is null, the notification is for all users
        public Guid? UserId { get; set; }
    }
}

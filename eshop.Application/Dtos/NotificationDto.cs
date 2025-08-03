namespace eshop.Application.Dtos
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public required string NotificationText { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}

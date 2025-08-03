
using eshop.Application.Dtos;

namespace eshop.Application.Contracts.Services
{
    public interface INotificationService
    {
        Task<bool> NotifyAllUsersAsync(string notificationText);
        Task<bool> NotifyUserAsync(Guid userId, string notificationText);
        Task<PagedList<NotificationDto>> GetUserNotificationsAsync(
            Guid userId, 
            bool includeRead = true,
            int pageNumber = 1, 
            int pageSize = 10);
        Task<bool> MarkUserNotificationAsReadAsync(Guid UserId, Guid notificationId);
    }
}

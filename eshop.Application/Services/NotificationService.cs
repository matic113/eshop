using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Application.Contracts.Services;
using eshop.Application.Dtos;
using eshop.Domain.Entities;
using System.Net.Http.Headers;

namespace eshop.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<PagedList<NotificationDto>> GetUserNotificationsAsync(Guid userId,
            bool includeRead = true,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return await _notificationRepository.GetUserNotificationsAsync(userId,
                includeRead,
                pageNumber,
                pageSize);
        }

        public async Task<bool> NotifyAllUsersAsync(string notificationText)
        {
            return await _notificationRepository.NotifyAllUsersAsync(notificationText);
        }

        public async Task<bool> NotifyUserAsync(Guid userId, string notificationText)
        {
            return await _notificationRepository.NotifyUserAsync(userId, notificationText);
        }

        public async Task<bool> MarkUserNotificationAsReadAsync(Guid UserId, Guid notificationId)
        {
            return await _notificationRepository.MarkUserNotificationAsReadAsync(UserId, notificationId);
        }
    }
}

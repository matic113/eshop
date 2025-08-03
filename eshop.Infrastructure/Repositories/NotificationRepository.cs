using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Application.Dtos;
using eshop.Domain.Entities;
using eshop.Infrastructure.Extensions;
using eshop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace eshop.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<NotificationDto>> GetUserNotificationsAsync(Guid userId,
            bool includeRead = true,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var notificationsQuery =
                from n in _context.Notifications
                where n.UserId == userId || n.UserId == null
                join un in _context.UserNotifications.Where(un => un.UserId == userId)
                on n.Id equals un.NotificationId into userNotifications
                from un in userNotifications.DefaultIfEmpty()

                select new NotificationDto
                {
                    Id = n.Id,
                    NotificationText = n.Text,
                    CreatedAt = n.CreatedAt,
                    IsRead = un != null && un.IsRead,
                };

            if (!includeRead)
            {
                notificationsQuery = notificationsQuery
                    .Where(n => !n.IsRead);
            }

            var sortedQuery = notificationsQuery
            .OrderByDescending(x => x.CreatedAt);

            return await sortedQuery.ToPagedListAsync(pageNumber, pageSize);
        }

        public async Task<bool> NotifyAllUsersAsync(string notificationText)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Text = notificationText,
                UserId = null // Null means this notification is for all users
            };

            await _context.AddAsync(notification);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> NotifyUserAsync(Guid userId, string notificationText)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                return false; // User does not exist
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Text = notificationText,
                UserId = userId // Specific user notification
            };

            await _context.AddAsync(notification);

            var userNotification = new UserNotification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                NotificationId = notification.Id
            };
            await _context.AddAsync(userNotification);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkUserNotificationAsReadAsync(Guid UserId, Guid notificationId)
        {
            var userNotification = await _context.UserNotifications
                .FirstOrDefaultAsync(un => un.UserId == UserId && un.NotificationId == notificationId);

            if (userNotification == null)
            {
                var newUserNotification = new UserNotification
                {
                    Id = Guid.NewGuid(),
                    UserId = UserId,
                    NotificationId = notificationId,
                    IsRead = true,
                    ReadAt = DateTime.UtcNow
                };

                await _context.UserNotifications.AddAsync(newUserNotification);
                await _context.SaveChangesAsync();
                return true;
            }

            if (userNotification.IsRead)
            {
                return false;
            }

            userNotification.IsRead = true;
            userNotification.ReadAt = DateTime.UtcNow;

            _context.UserNotifications.Update(userNotification);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

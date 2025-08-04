using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Extensions;
using FastEndpoints;

namespace eshop.API.Features.Notifications
{
    public class BulkMarkAsRead
    {
        sealed class BulkMarkNotificationsAsReadRequest
        {
            public List<Guid> NotificationIds { get; set; } = new List<Guid>();
        }

        sealed class BulkMarkNotificationsAsReadResponse
        {
            public string? Message { get; set; }
        }

        sealed class BulkMarkNotificationsAsReadEndpoint : Endpoint<BulkMarkNotificationsAsReadRequest, BulkMarkNotificationsAsReadResponse>
        {
            private readonly INotificationService _notificationService;

            public BulkMarkNotificationsAsReadEndpoint(INotificationService notificationService)
            {
                _notificationService = notificationService;
            }

            public override void Configure()
            {
                Post("/api/notifications/bulk-read");
                Description(x => x
                    .WithTags("Notifications")
                    .WithDescription("Bulk mark notifications as read with notifications ids.")
                    .WithName("BulkMarkNotificationsAsRead")
                    .ProducesProblem(401)
                    .Produces<BulkMarkNotificationsAsReadResponse>());
            }

            public override async Task HandleAsync(BulkMarkNotificationsAsReadRequest r, CancellationToken c)
            {
                var userId = User.GetUserId();
                if (userId == null)
                {
                    await SendUnauthorizedAsync();
                    return;
                }

                if (r.NotificationIds == null || !r.NotificationIds.Any())
                {
                    AddError(x => x.NotificationIds,
                        "Notification IDs cannot be empty.");

                    await SendErrorsAsync();
                    return;
                }

                var result = await _notificationService
                    .MarkBulkUserNotificationsAsReadAsync(userId.Value, r.NotificationIds);

                if (!result)
                {
                    AddError("Failed to mark notifications as read. Some or all notifications may already be marked as read.");
                    await SendErrorsAsync();
                    return;
                }

                await SendOkAsync(new BulkMarkNotificationsAsReadResponse
                {
                    Message = "Notifications marked as read successfully."
                });
            }
        }
    }
}

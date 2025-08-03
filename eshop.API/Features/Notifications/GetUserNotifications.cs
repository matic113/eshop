using eshop.Application.Contracts.Services;
using eshop.Application.Dtos;
using eshop.Infrastructure.Extensions;
using FastEndpoints;

namespace eshop.API.Features.Notifications
{
    public class GetUserNotifications
    {
        sealed class GetUserNotificationsRequest
        {
            public bool IncludeRead { get; set; } = true;
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }

        sealed class GetUserNotificationsResponse
        {
            public PagedList<NotificationDto> Notifications { get; set; } = null!;
        }

        sealed class GetUserNotificationsEndpoint : Endpoint<GetUserNotificationsRequest, GetUserNotificationsResponse>
        {
            private readonly INotificationService _notificationService;

            public GetUserNotificationsEndpoint(INotificationService notificationService)
            {
                _notificationService = notificationService;
            }

            public override void Configure()
            {
                Get("/api/notifications");
                Description(x => x
                    .WithTags("Notifications")
                    .WithDescription("Get all user notifications")
                    .WithName("GetUserNotification")
                    .Produces<GetUserNotificationsResponse>());
            }

            public override async Task HandleAsync(GetUserNotificationsRequest r, CancellationToken c)
            {
                var userId = User.GetUserId();

                if (userId == null)
                {
                    await SendUnauthorizedAsync();
                    return;
                }

                var notifications = await _notificationService.GetUserNotificationsAsync(
                    userId.Value,
                    r.IncludeRead,
                    r.Page,
                    r.PageSize);

                await SendOkAsync(new GetUserNotificationsResponse
                {
                    Notifications = notifications
                });
            }
        }
    }
}

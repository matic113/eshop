using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace eshop.API.Features.Notifications
{
    public class MarkAsRead
    {
        sealed class MarkNotificationAsReadResponse
        {
            public string? Message { get; set; }
        }

        sealed class MarkNotificationAsReadEndpoint : EndpointWithoutRequest<MarkNotificationAsReadResponse>
        {
            private readonly INotificationService _notificationService;

            public MarkNotificationAsReadEndpoint(INotificationService notificationService)
            {
                _notificationService = notificationService;
            }

            public override void Configure()
            {
                Post("/api/notifications/{notificationId}/read");
                Description(x => x
                    .WithTags("Notifications")
                    .WithDescription("Mark a notification as read")
                    .WithName("MarkNotificationAsRead")
                    .Produces<MarkNotificationAsReadResponse>());
            }

            public override async Task HandleAsync(CancellationToken c)
            {
                var userId = User.GetUserId();

                if (userId == null)
                {
                    await SendUnauthorizedAsync();
                    return;
                }

                var notificationId = Route<Guid>("notificationId");

                var result = await _notificationService
                    .MarkUserNotificationAsReadAsync(userId.Value, notificationId);

                if (!result)
                {
                    AddError("Notification already marked as read.");
                    await SendErrorsAsync();
                    return;
                }

                await SendOkAsync(new MarkNotificationAsReadResponse
                {
                    Message = "Notification marked as read successfully."
                });
            }
        }
    }
}

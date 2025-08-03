using eshop.Application.Contracts.Services;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Notifications
{
    public class NotifyAll
    {
        sealed class NotifyAllUsersRequest
        {
            public string NotificationText { get; set; } = string.Empty;
        }
        sealed class NotifyAllUsersValidator : Validator<NotifyAllUsersRequest>
        {
            public NotifyAllUsersValidator()
            {
                RuleFor(x => x.NotificationText)
                    .NotEmpty().WithMessage("Notification text cannot be empty.")
                    .MaximumLength(250).WithMessage("Notification text cannot exceed 250 characters.");
            }
        }
        sealed class NotifyAllUsersResponse
        {
            public string Message{ get; set; } = string.Empty;
        }

        sealed class NotifyAllUsersEndpoint : Endpoint<NotifyAllUsersRequest, NotifyAllUsersResponse>
        {
            private readonly INotificationService _notificationService;

            public NotifyAllUsersEndpoint(INotificationService notificationService)
            {
                _notificationService = notificationService;
            }

            public override void Configure()
            {
                Post("/api/notifications/all");
                Description(x => x
                    .WithTags("Notifications")
                    .WithDescription("Sends a notification to all users.")
                    .WithName("NotifyAllUsers")
                    .Produces<NotifyAllUsersResponse>());
            }

            public override async Task HandleAsync(NotifyAllUsersRequest r, CancellationToken c)
            {
                var result = await _notificationService.NotifyAllUsersAsync(r.NotificationText);

                if (!result)
                {
                    AddError("Failed to notify all users.");
                    await SendErrorsAsync();
                    return;
                }

                await SendOkAsync(new NotifyAllUsersResponse
                {
                    Message = "All users have been notified successfully."
                });
            }
        }
    }
}

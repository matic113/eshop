using eshop.Application.Contracts.Services;
using eshop.Application.Dtos;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Notifications
{
    public class NotifiyUser
    {
        sealed class NotifyUserRequest
        {
            public Guid UserId { get; set; }
            public string NotificationText { get; set; } = string.Empty;
        }

        sealed class NotifyUserValidator : Validator<NotifyUserRequest>
        {
            public NotifyUserValidator()
            {
                RuleFor(x => x.UserId)
                    .NotEmpty().WithMessage("User ID cannot be empty.");
                
                RuleFor(x => x.NotificationText)
                    .NotEmpty().WithMessage("Notification text cannot be empty.")
                    .MaximumLength(250).WithMessage("Notification text cannot exceed 250 characters.");
            }
        }

        sealed class NotifyUserResponse
        {
            public string Message { get; set; } = string.Empty;
        }

        sealed class NotifyUserEndpoint : Endpoint<NotifyUserRequest, NotifyUserResponse>
        {
            private readonly INotificationService _notificationService;

            public NotifyUserEndpoint(INotificationService notificationService)
            {
                _notificationService = notificationService;
            }

            public override void Configure()
            {
                Post("/api/notifications");
                Description(x => x
                    .WithTags("Notifications")
                    .WithDescription("Sends a notification to a specific user.")
                    .WithName("NotifyUser")
                    .Produces<NotifyUserResponse>());
            }

            public override async Task HandleAsync(NotifyUserRequest r, CancellationToken c)
            {
                var result = await _notificationService.NotifyUserAsync(r.UserId, r.NotificationText);

                if (!result)
                {
                    AddError("User doesn't exist or an error occured whilte trying to notify user.");
                    await SendErrorsAsync();
                    return;
                }

                await SendOkAsync(new NotifyUserResponse
                {
                    Message = "User has been notified successfully."
                });
            }
        }
    }
}

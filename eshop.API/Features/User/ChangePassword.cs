using eshop.Infrastructure.Persistence;
using System.Security.Claims;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace eshop.API.Features.User
{
    public class ChangePassword
    {
        public sealed class ChangePasswordRequest
        {
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmNewPassword { get; set; }
        }
        public sealed class ChangePasswordValidator : Validator<ChangePasswordRequest>
        {
            public ChangePasswordValidator()
            {
                RuleFor(x => x.CurrentPassword)
                    .NotEmpty().WithMessage("Current password is required.");

                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithMessage("New password is required.")
                    .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                    .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
                    .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");

                RuleFor(x => x.ConfirmNewPassword)
                    .Equal(x => x.NewPassword).WithMessage("The new password and confirmation password do not match.");
            }
        }
        public sealed class ChangePasswordEndpoint : Endpoint<ChangePasswordRequest>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public ChangePasswordEndpoint(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }
            public override void Configure()
            {
                Post("/api/auth/change-password");
                // This endpoint is NOT anonymous. The user must be logged in.
                Description(x => x
                    .WithName("ChangePassword")
                    .WithTags("auth")
                    .Produces(200)
                    .Produces(401)
                    .Produces(400));
            }

            public override async Task HandleAsync(ChangePasswordRequest r, CancellationToken c)
            {
                // 1. Get the current user's ID from their authentication token (JWT).
                // This is secure because the token cannot be forged.
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdString))
                {
                    await SendUnauthorizedAsync(c);
                    return;
                }

                var user = await _userManager.FindByIdAsync(userIdString);
                if (user is null)
                {
                    await SendUnauthorizedAsync(c);
                    return;
                }

                // 2. Use the built-in Identity method to securely change the password.
                // This method handles checking the current password for you.
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, r.CurrentPassword, r.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    // Add all errors from Identity to the validation response.
                    foreach (var error in changePasswordResult.Errors)
                    {
                        AddError(error.Description);
                    }
                    await SendErrorsAsync(cancellation: c);
                    return;
                }

                await SendOkAsync("Password has been changed successfully.", c);
            }
        }
    }
}

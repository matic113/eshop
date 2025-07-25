using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Persistence;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace eshop.API.Features.User
{
    public class ResetPassword
    {
        sealed class ResetPasswordRequest
        {
            public required string Email { get; set; }
            public required string Otp { get; set; }
            public required string NewPassword { get; set; }
        }

        sealed class ResetPasswordValidator : Validator<ResetPasswordRequest>
        {
            public ResetPasswordValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email address is required.")
                    .EmailAddress().WithMessage("Invalid email address format.");
                RuleFor(x => x.Otp)
                    .NotEmpty().WithMessage("OTP is required.")
                    .Length(6).WithMessage("OTP must be 6 digits long.");
                RuleFor(x => x.NewPassword)
                    .NotEmpty()
                    .WithMessage("Password is required.")
                    .MinimumLength(8)
                    .WithMessage("Password must be at least 8 characters.")
                    .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
                    .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");
            }
        }

        sealed class ResetPasswordEndpoint : Endpoint<ResetPasswordRequest>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IOtpService _otpService;
            private readonly IEmailService _emailService;

            public ResetPasswordEndpoint(IOtpService otpService, UserManager<ApplicationUser> userManager, IEmailService emailService)
            {
                _otpService = otpService;
                _userManager = userManager;
                _emailService = emailService;
            }

            public override void Configure()
            {
                Post("/api/auth/reset-password");
                AllowAnonymous();
                Description(x => x
                    .WithName("ResetPassword")
                    .WithTags("Auth")
                    .Produces(200)
                    .ProducesProblem(400)
                    .ProducesProblem(404));
            }

            public override async Task HandleAsync(ResetPasswordRequest r, CancellationToken c)
            {
                // 1. Get the userId to use it in validating OTP

                var user = await _userManager.FindByEmailAsync(r.Email);

                if (user is null)
                {
                    AddError(x => x.Email, "User with this email address didn't request a password change.");
                    await SendErrorsAsync(cancellation: c);
                    return;
                }

                var isValidOtp = await _otpService.ValidateOtpAsync(user.Id, r.Otp);

                if (!isValidOtp)
                {
                    AddError(x => x.Otp, "Invalid OTP or No user is registered with this email.");
                    await SendErrorsAsync(cancellation: c);
                    return;
                }

                // 2. Reset the password
                //      1. Remove the old password.

                var removeResult = await _userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                {
                    AddError("an error occured while removing the old password.");
                    await SendErrorsAsync(cancellation: c);
                    return;
                }

                //      2. Add the new password.
                var addResult = await _userManager.AddPasswordAsync(user, r.NewPassword);
                if (!addResult.Succeeded)
                {
                    AddError("an error occured whule adding the new password");
                    await SendErrorsAsync(cancellation: c);
                    return;
                }

                // 3. Send confirmation email
                await _emailService.SendPasswordResetConfirmationEmailAsync(user.Email!, user.FirstName);

                // 4. Clean up the OTP
                await _otpService.DeleteTokenByUserIdAsync(user.Id);

                await SendOkAsync("Password has been reset successfully.");
            }
        }
    }
}

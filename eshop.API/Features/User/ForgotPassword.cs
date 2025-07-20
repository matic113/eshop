using eshop.Application.Contracts;
using eshop.Infrastructure.Persistence;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace eshop.API.Features.User
{
    public class ForgotPassword
    {
        sealed class ResetPasswordRequest
        {
            public required string Email { get; set; }
        }

        sealed class ValidateResetPasswordRequest : Validator<ResetPasswordRequest>
        {
            public ValidateResetPasswordRequest()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email address is required.")
                    .EmailAddress().WithMessage("Invalid email address format.");
            }
        }

        sealed class ResetPasswordEndpoint : Endpoint<ResetPasswordRequest>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IOtpService _otpService;

            public ResetPasswordEndpoint(IOtpService otpService, UserManager<ApplicationUser> userManager)
            {
                _otpService = otpService;
                _userManager = userManager;
            }

            public override void Configure()
            {
                Post("/api/auth/forgot-password");
                AllowAnonymous();
            }

            public override async Task HandleAsync(ResetPasswordRequest r, CancellationToken c)
            {
                var user = await _userManager.FindByEmailAsync(r.Email);

                if (user is not null)
                {
                    var otp = await _otpService.GenerateOtpAsync(user.Id);

                    await _otpService.SendResetPasswordOtpEmailAsync(otp, user.Email!);
                }

                // Regardless of whether the user exists or not, we do not want to reveal if the email is registered.

                await SendOkAsync("If the email is registered, you will receive an OTP to reset your password.", c);
            }
        }
    }
}

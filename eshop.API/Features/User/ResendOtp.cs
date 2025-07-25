using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Persistence;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace eshop.API.Features.User
{
    public class ResendOtp
    {
        sealed class ResendOtpRequest
        {
            public required string Email { get; set; }
        }
        sealed class ResendOtpValidator : Validator<ResendOtpRequest>
        {
            public ResendOtpValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email address is required.")
                    .EmailAddress().WithMessage("Invalid email address format.");
            }
        }

        sealed class ResendOtpEndpoint : Endpoint<ResendOtpRequest>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IOtpService _otpService;
            private readonly IEmailService _emailService;

            public ResendOtpEndpoint(UserManager<ApplicationUser> userManager, IOtpService otpService, IEmailService emailService)
            {
                _userManager = userManager;
                _otpService = otpService;
                _emailService = emailService;
            }

            public override void Configure()
            {
                Post("/api/auth/resend-otp");
                AllowAnonymous();
                Throttle(1, 60); // Limit to 1 request per minute
                Description(x => x
                    .WithName("ResendOtp")
                    .WithTags("Auth")
                    .Produces(200));
            }

            public override async Task HandleAsync(ResendOtpRequest r, CancellationToken c)
            {
                var user = await _userManager.FindByEmailAsync(r.Email);

                if (user is not null)
                {
                    var otp = await _otpService.GenerateNewOtpAsync(user.Id);
                    await _emailService.SendOtpEmailAsync(user.Email!, otp, user.FirstName);
                }

                // Regardless of whether the user exists or not, we do not want to reveal if the email is registered.

                await SendOkAsync("OTP has been resent to your email.", c);
            }
        }
    }
}

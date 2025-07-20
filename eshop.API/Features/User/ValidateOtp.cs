using eshop.Application.Contracts;
using eshop.Infrastructure.Persistence;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace eshop.API.Features.User
{
    public class ValidateOtp
    {
        sealed class ValidateOtpRequest
        {
            public required string Email { get; set; }
            public required string Otp { get; set; }
        }
        sealed class ValidateOtpValidator : Validator<ValidateOtpRequest>
        {
            public ValidateOtpValidator()
            {
                RuleFor(x => x.Otp)
                    .NotEmpty().WithMessage("OTP is required.")
                    .Length(6).WithMessage("OTP must be 6 digits long.");
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email address is required.")
                    .EmailAddress().WithMessage("Invalid email address format.");
            }
        }
        sealed class ValidateOtpEndpoint : Endpoint<ValidateOtpRequest>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IOtpService _otpService;
            public ValidateOtpEndpoint(IOtpService otpService, UserManager<ApplicationUser> userManager)
            {
                _otpService = otpService;
                _userManager = userManager;
            }
            public override void Configure()
            {
                Post("/api/auth/validate-otp");
                AllowAnonymous();
                Description(x => x
                    .WithName("ValidateOtp")
                    .WithTags("Auth")
                    .Produces(200)
                    .Produces(400));
            }
            public override async Task HandleAsync(ValidateOtpRequest r, CancellationToken c)
            {
                var user = await _userManager.FindByEmailAsync(r.Email);

                if (user is null)
                {
                    AddError(x => x.Otp, "Invalid OTP or Email.");
                    await SendErrorsAsync();
                    return;
                }

                var isValid = await _otpService.ValidateOtpAsync(user.Id, r.Otp);

                if (isValid)
                {
                    await SendOkAsync("OTP is valid.", c);
                }
                else
                {
                    AddError(x => x.Otp, "Invalid OTP or Email.");
                    await SendErrorsAsync();
                }
            }
        }
    }
}

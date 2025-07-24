using eshop.Infrastructure.Persistence;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eshop.API.Features.User
{
    public record VerifyEmailRequest(string Email, string Otp);
    public class VerifyEmailRequestValidator : Validator<VerifyEmailRequest>
    {
        public VerifyEmailRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Email is not valid.");

            RuleFor(x => x.Otp)
                .Length(6)
                .WithMessage("Otp must be 6 characters long.");
        }
    }
    public class VerifyEmail : Endpoint<VerifyEmailRequest>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VerifyEmail(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public override void Configure()
        {
            Post("/api/auth/verify-email");
            AllowAnonymous();
            Description(x => x
                .WithTags("Auth"));
        }

        public override async Task HandleAsync(VerifyEmailRequest req, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);

            if (user == null)
            {
                AddError(x => x.Email, "User with this email does not exist.");
                await SendErrorsAsync(cancellation: ct);
                return;
            }

            var verificationToken = await _context.VerificationTokens
                .FirstOrDefaultAsync(t => t.UserId == user.Id && t.Code == req.Otp, ct);

            if (verificationToken == null)
            {
                AddError(x => x.Otp, "Invalid OTP.");
                await SendErrorsAsync(cancellation: ct);
                return;
            }

            if (verificationToken.IsExpired)
            {
                AddError(x => x.Otp, "OTP has expired.");
                await SendErrorsAsync(cancellation: ct);
                return;
            }

            user.EmailConfirmed = true;
            _context.VerificationTokens.Remove(verificationToken);

            await _context.SaveChangesAsync(ct);

            await SendOkAsync("Email verified successfully.", cancellation: ct);
        }
    }
}

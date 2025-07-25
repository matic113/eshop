using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Persistence;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Features.User;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName);

public class RegisterValidator : Validator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email is not valid.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");


        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MinimumLength(3)
            .WithMessage("First name must be at least 3 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MinimumLength(3)
            .WithMessage("Last name must be at least 3 characters.");

    }
}

public class Register : Endpoint<RegisterRequest>
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;

    public Register(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOtpService otpService, IEmailService emailService)
    {
        _context = context;
        _userManager = userManager;
        _otpService = otpService;
        _emailService = emailService;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
        Description(x => x.
            WithTags("Auth"));
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        // first check if the email already used

        var userExists = await _userManager.FindByEmailAsync(req.Email) != null;

        if (userExists)
        {
            AddError(x => x.Email, "Email is already in use.");
            await SendErrorsAsync();
            return;
        }

        var user = ApplicationUser.Create(req.Email, req.FirstName, req.LastName);

        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            var errors = (result.Errors.Select(x => x.Description));
            AddError("Registration", string.Join(", ", errors));
            await SendErrorsAsync();
            return;
        }

        var otp = await _otpService.GenerateOtpAsync(user.Id);

        await _emailService.SendOtpEmailAsync(req.Email, otp, user.FirstName);

        await SendOkAsync(new
        {
            Message = "Registeration Successfull, an otp is sent to your email inbox please use it to verify your email."
        }, ct);
    }
}
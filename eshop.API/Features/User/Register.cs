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

    public Register(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
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

        await SendOkAsync(new
        {
            Message = "User registered successfully."
        }, ct);
    }
}
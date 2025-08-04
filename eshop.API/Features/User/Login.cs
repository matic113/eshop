using Auth.API.Extensions;
using eshop.Infrastructure.Authentication;
using eshop.Infrastructure.Persistence;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Features.User;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, string RefreshToken);

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
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
    }
}

public class Login : Endpoint<LoginRequest, LoginResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtService _jwtService;

    public Login(ApplicationDbContext context, UserManager<ApplicationUser> userManager, JwtService jwtService)
    {
        _context = context;
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Description(x => x.
            WithTags("Auth").
            Produces<LoginResponse>(200));
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, req.Password))
        {
            AddError("Invalid email or password.");
            await SendErrorsAsync();
            return;
        }

        if (!user.EmailConfirmed)
        {
            AddError(x => x.Email, "Email not verified, please verify your email first.");
            await SendErrorsAsync();
            return;
        }

        var (jwtToken, expirationDateInUtc) = _jwtService.GenerateJwtToken(user);

        var refreshTokenValue = _jwtService.GenerateRefreshToken();
        var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

        user.RefreshToken = refreshTokenValue.HashedToken();
        user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

        await _userManager.UpdateAsync(user);

        #region Http-Cookie
        // Optionally, you can set the tokens in http-only cookies if needed.

        var httpResponse = HttpContext.Response;

        var accessTokenCookieOptions = new CookieOptions
        {
            HttpOnly = true, // Makes the cookie inaccessible to client-side script
            Expires = expirationDateInUtc,
            Secure = true, // Transmit the cookie only over HTTPS
            SameSite = SameSiteMode.None
        };
        httpResponse.Cookies.Append("access_token", jwtToken, accessTokenCookieOptions);

        // Cookie options for the Refresh Token
        var refreshTokenCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshTokenExpirationDateInUtc,
            Secure = true,
            SameSite = SameSiteMode.None,
        };
        httpResponse.Cookies.Append("refresh_token", refreshTokenValue, refreshTokenCookieOptions);
        #endregion

        var response = new LoginResponse(jwtToken, expirationDateInUtc, refreshTokenValue);

        await SendOkAsync(response);
    }
}
using Auth.API.Extensions;
using eshop.Infrastructure.Authentication;
using eshop.Infrastructure.Persistence;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Features.User
{
    public record RefreshTokenRequest(string? RefreshToken, bool UseCookies = false);
    public record RefreshTokenResponse(string AccessToken, DateTime ExpiresAtUtc, string RefreshToken);
    public class RefreshTokenValidator : Validator<RefreshTokenRequest>
    {
        public RefreshTokenValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .When(x => !x.UseCookies)
                .WithMessage("Refresh token is required.");
        }
    }
    public class RefreshToken : Endpoint<RefreshTokenRequest, RefreshTokenResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;
        public RefreshToken(ApplicationDbContext context, UserManager<ApplicationUser> userManager, JwtService jwtService)
        {
            _context = context;
            _userManager = userManager;
            _jwtService = jwtService;
        }
        public override void Configure()
        {
            Post("/api/auth/refresh-token");
            AllowAnonymous();
            Description(x => x.
                WithTags("Auth").
                Produces<RefreshTokenResponse>(200));
        }
        public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(req.RefreshToken) && !req.UseCookies)
            {
                AddError(x => x.RefreshToken, "Refresh token is required.");
                await SendErrorsAsync(cancellation: ct);
                return;
            }
            
            var refreshToken = req.UseCookies ? 
                HttpContext.Request.Cookies["refresh_token"] : 
                req.RefreshToken;

            var hashedToken = refreshToken?.HashedToken();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == hashedToken, ct);

            if (user == null)
            {
                AddError(x => x.RefreshToken, "Invalid refresh token.");
                await SendErrorsAsync(cancellation: ct);
                return;
            }

            if (user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
            {
                AddError(x => x.RefreshToken, "Refresh token has expired.");
                await SendErrorsAsync(cancellation: ct);
                return;
            }

            var (jwtToken, expirationDateInUtc) = _jwtService.GenerateJwtToken(user);

            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = newRefreshToken.HashedToken();
            user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

            await _userManager.UpdateAsync(user);

            if (req.UseCookies)
            {
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
                httpResponse.Cookies.Append("refresh_token", newRefreshToken, refreshTokenCookieOptions);
            }


            var response = new RefreshTokenResponse(jwtToken, expirationDateInUtc, newRefreshToken);
            await SendAsync(response, cancellation: ct);
        }
    }
}

using System.Security.Claims;
using Auth.API.Extensions;
using eshop.Infrastructure.Authentication;
using eshop.Infrastructure.Persistence;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Features.Google
{
    public record CallbackRequest(string returnUrl);
    public record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, string RefreshToken);
    public class Callback : Endpoint<CallbackRequest>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;

        public Callback(UserManager<ApplicationUser> userManager, JwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }
        public override void Configure()
        {
            Get("/api/auth/google/callback");
            AllowAnonymous();
            Description(x => x
            .WithName("GoogleLoginCallback")
            .WithTags("GoogleAuth"));
        }
        public override async Task HandleAsync(CallbackRequest req, CancellationToken ct)
        {
            var authResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authResult.Succeeded)
            {
                await SendUnauthorizedAsync(cancellation: ct);
                return;
            }

            var claimsPrincipal = authResult.Principal;

            if (claimsPrincipal == null)
            {
                await SendUnauthorizedAsync(cancellation: ct);
                return;
            }

            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            if (email is null)
            {
                await SendUnauthorizedAsync(cancellation: ct);
                return;
            }
            var user = await _userManager.FindByEmailAsync(email);

            var profilePicture = claimsPrincipal.FindFirstValue("picture");

            if (user == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                    LastName = claimsPrincipal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
                    EmailConfirmed = true,
                    ProfilePicture = profilePicture,
                };

                var result = await _userManager.CreateAsync(newUser);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    AddError("Registration: ", errors);
                    await SendErrorsAsync(cancellation: ct);
                    return;
                }

                user = newUser;
            }

            // Update user picture if it has changed
            user.ProfilePicture = user.ProfilePicture != profilePicture ? profilePicture : user.ProfilePicture;

            // Check if the external login already exists
            var existingLogin = await _userManager.FindByLoginAsync("Google", claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? email);

            if (existingLogin == null)
            {
                var info = new UserLoginInfo("Google", claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? email, "Google");

                var loginResult = await _userManager.AddLoginAsync(user, info);

                if (!loginResult.Succeeded)
                {
                    var errors = string.Join(", ", loginResult.Errors.Select(e => e.Description));
                    AddError("Login: ", errors);
                    await SendErrorsAsync(cancellation: ct);
                    return;
                }
            }

            var (jwtToken, expirationDateInUtc) = _jwtService.GenerateJwtToken(user);

            var refreshTokenValue = _jwtService.GenerateRefreshToken();
            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshTokenValue.HashedToken();
            user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

            await _userManager.UpdateAsync(user);
            
            var httpResponse = HttpContext.Response;

            var accessTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true, // Makes the cookie inaccessible to client-side script
                Expires = expirationDateInUtc,
                Secure = true, // Transmit the cookie only over HTTPS
                SameSite = SameSiteMode.Lax
            };
            httpResponse.Cookies.Append("access_token", jwtToken, accessTokenCookieOptions);

            // Cookie options for the Refresh Token
            var refreshTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshTokenExpirationDateInUtc,
                Secure = true,
                SameSite = SameSiteMode.Lax,
            };
            httpResponse.Cookies.Append("refresh_token", refreshTokenValue, refreshTokenCookieOptions);

            await SendRedirectAsync(req.returnUrl, allowRemoteRedirects: true);
        }
    }
}

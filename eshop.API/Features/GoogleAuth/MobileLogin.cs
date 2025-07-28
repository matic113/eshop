using Auth.API.Extensions;
using eshop.Infrastructure.Authentication;
using eshop.Infrastructure.Persistence;
using FastEndpoints;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Features.Google
{
    record MobileLoginRequest(string IdToken);
    sealed class MobileSignIn : Endpoint<MobileLoginRequest, LoginResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;

        public MobileSignIn(
            UserManager<ApplicationUser> userManager,
            JwtService jwtService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        public override void Configure()
        {
            Post("/api/auth/google/mobile");
            AllowAnonymous();
            Description(x => x
                .WithTags("GoogleAuth")
                .WithSummary("Handles google login from a mobile device using an Id Token."));
        }

        public override async Task HandleAsync(MobileLoginRequest req, CancellationToken ct)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var validAudiences = new[] {
                    _configuration["Authentication:Google:AndroidClientId"]
                };

                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    // The audience must be one of the client IDs from your Google Cloud project.
                    Audience = validAudiences
                };

                // Validate the token and get the user's payload
                payload = await GoogleJsonWebSignature.ValidateAsync(req.IdToken, validationSettings);
            }
            catch (InvalidJwtException ex)
            {
                Logger.LogWarning(ex, "Invalid Google ID token received.");
                await SendUnauthorizedAsync(cancellation: ct);
                return;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An unexpected error occurred during Google token validation.");
                AddError("An unexpected error occurred during authentication.");
                await SendErrorsAsync(500, ct);
                return;
            }

            // At this point, the token is valid. Now, use the payload to find or create a local user.
            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    FirstName = payload.GivenName ?? string.Empty,
                    LastName = payload.FamilyName ?? string.Empty,
                    EmailConfirmed = payload.EmailVerified // Use the value from Google's token
                };

                var result = await _userManager.CreateAsync(newUser);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    AddError($"User creation failed: {errors}");
                    await SendErrorsAsync(cancellation: ct);
                    return;
                }

                user = newUser;
            }

            // Check if this external login is already associated with the user.
            var existingLogin = await _userManager.FindByLoginAsync("Google", payload.Subject);

            if (existingLogin == null)
            {
                // The "Subject" from the payload is Google's unique ID for the user.
                var info = new UserLoginInfo("Google", payload.Subject, "Google");

                var loginResult = await _userManager.AddLoginAsync(user, info);

                if (!loginResult.Succeeded)
                {
                    var errors = string.Join(", ", loginResult.Errors.Select(e => e.Description));
                    AddError($"Failed to add Google login: {errors}");
                    await SendErrorsAsync(cancellation: ct);
                    return;
                }
            }

            // Now that we have a valid local user, generate our own tokens.
            var (jwtToken, expirationDateInUtc) = _jwtService.GenerateJwtToken(user);
            var refreshTokenValue = _jwtService.GenerateRefreshToken();
            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshTokenValue.HashedToken();
            user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

            await _userManager.UpdateAsync(user);

            var response = new LoginResponse(jwtToken, expirationDateInUtc, refreshTokenValue);

            await SendOkAsync(response, cancellation: ct);
        }
    }
}

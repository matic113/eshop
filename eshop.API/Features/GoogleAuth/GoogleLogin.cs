using eshop.Infrastructure.Persistence;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Features.Google
{
    public record GoogleLoginRequest(string returnUrl);
    public class Login : Endpoint<GoogleLoginRequest>
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public Login(LinkGenerator linkGenerator, SignInManager<ApplicationUser> signInManager)
        {
            _linkGenerator = linkGenerator;
            _signInManager = signInManager;
        }
        public override void Configure()
        {
            Get("/api/auth/google/login");
            AllowAnonymous();
            DontAutoSendResponse();
            Description(x => x
                .WithTags("GoogleAuth"));
        }
        public override async Task HandleAsync(GoogleLoginRequest req, CancellationToken ct)
        {
            var callbackUrl = _linkGenerator.GetPathByName(HttpContext, "GoogleLoginCallback");

            if (!string.IsNullOrEmpty(req.returnUrl))
                callbackUrl += $"?returnUrl={Uri.EscapeDataString(req.returnUrl)}";

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", callbackUrl);

            await HttpContext.ChallengeAsync("Google", properties);
        }
    }
}

using eshop.Infrastructure.Persistence;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Features.Google
{
    public record LoginRequest(string returnUrl);
    public class Login : Endpoint<LoginRequest>
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
        }
        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            var callbackUrl = _linkGenerator.GetPathByName(HttpContext, "GoogleLoginCallback");

            if (!string.IsNullOrEmpty(req.returnUrl))
                callbackUrl += $"?returnUrl={Uri.EscapeDataString(req.returnUrl)}";

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", callbackUrl);

            await HttpContext.ChallengeAsync("Google", properties);
        }
    }
}

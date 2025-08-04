using FastEndpoints;

namespace eshop.API.Features.User
{
    public class Logout
    {
        sealed class LogoutEndpoint : EndpointWithoutRequest
        {
            public override void Configure()
            {
                Post("/api/auth/logout");
                Description(x => x
                    .WithTags("Auth")
                    .WithDescription("Sets access_token and refresh_token cookies to empty.")
                    );
            }

            public override async Task HandleAsync(CancellationToken c)
            {
                var httpRespone = HttpContext.Response;

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(-1)
                };

                // Clear access token cookie
                httpRespone.Cookies.Append("access_token", string.Empty, cookieOptions);
                httpRespone.Cookies.Append("refresh_token", string.Empty, cookieOptions);

                await SendOkAsync();
            }
        }
    }
}

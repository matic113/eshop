using eshop.Infrastructure.Extensions;
using eshop.Infrastructure.Persistence;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace eshop.API.Features.User
{
    public class Me
    {
        sealed class CurrentUserInfoResponse
        {
            public Guid UserId { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
        }
        sealed class CurrentUserInfoEndpoint : EndpointWithoutRequest<CurrentUserInfoResponse>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public CurrentUserInfoEndpoint(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }

            public override void Configure()
            {
                Get("/api/auth/me");
                Description(x => x
                    .WithName("GetCurrentUserInfo")
                    .WithTags("Auth")
                    .Produces<CurrentUserInfoResponse>(200)
                    .ProducesProblem(401)
                    .ProducesProblem(404));
            }

            public override async Task HandleAsync(CancellationToken c)
            {
                var currentUserId = User.GetUserId();

                if (currentUserId is null)
                {
                    await SendUnauthorizedAsync();
                    return;
                }

                var user = await _userManager.FindByIdAsync(currentUserId.Value.ToString());

                if (user is null)
                {
                    await SendNotFoundAsync(c);
                    return;
                }

                var response = new CurrentUserInfoResponse
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.ToString() ?? string.Empty
                };

                await SendOkAsync(response, c);
            }
        }
    }
}

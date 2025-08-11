using eshop.Infrastructure.Authentication;
using FastEndpoints;

namespace eshop.API.Features.User
{
    public class CheckAdmin
    {
        sealed class CheckAdminRoleEndpoint : EndpointWithoutRequest
        {
            public override void Configure()
            {
                Get("/api/auth/admin");
                Roles(ApplicationRoles.Admin);
                Description(x => x
                    .WithTags("Users")
                    .WithDescription("Checks if the user has admin role.")
                );
            }

            public override async Task HandleAsync(CancellationToken c)
            {
                await SendOkAsync("User is an admin.");
            }
        }
    }
}

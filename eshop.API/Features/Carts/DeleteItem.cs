using eshop.Application.Contracts;
using eshop.Infrastructure.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace eshop.API.Features.Carts
{
    public class DeleteItem
    {
        sealed class DeleteItemFromCartRequest
        {
            [FromRoute]
            public Guid Id { get; set; }
        }

        sealed class DeleteItemFromCartEndpoint : Endpoint<DeleteItemFromCartRequest>
        {
            private readonly ICartRepository _cartRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteItemFromCartEndpoint(ICartRepository cartRepository, IUnitOfWork unitOfWork)
            {
                _cartRepository = cartRepository;
                _unitOfWork = unitOfWork;
            }

            public override void Configure()
            {
                Delete("/api/cart/items/{Id}");
                Description(x =>
                {
                    x.Produces(200);
                    x.Produces(401);
                    x.Produces(404);
                    x.WithTags("Cart");
                });
            }

            public override async Task HandleAsync(DeleteItemFromCartRequest r, CancellationToken c)
            {
                var userId = User.GetUserId();
                
                if (userId is null)
                {
                    await SendUnauthorizedAsync(c);
                    return;
                }
                bool result = await _cartRepository.RemoveItemFromCartAsync(userId.Value, r.Id);
                await _unitOfWork.SaveChangesAsync(c);

                if (result)
                {
                    await SendOkAsync(c);
                }
                else
                {
                    await SendNotFoundAsync(c);
                }
            }
        }
    }
}

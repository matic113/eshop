using eshop.Application.Contracts;
using eshop.Domain.Entities;
using eshop.Infrastructure.Extensions;
using FastEndpoints;

namespace eshop.API.Features.Carts
{
    public class Get
    {
        sealed class GetCartResponse
        {
            public Guid CartId { get; set; }
            public List<CartItemResponse> CartItems { get; set; } = new List<CartItemResponse>();
        }
        sealed class CartItemResponse
        {
            public Guid ItemId { get; set; }
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }

        sealed class GetCartEndpoint : EndpointWithoutRequest<GetCartResponse>
        {
            private readonly ICartRepository _cartRepository;

            public GetCartEndpoint(ICartRepository cartRepository)
            {
                _cartRepository = cartRepository;
            }

            public override void Configure()
            {
                Get("/api/cart");
                Description(x =>
                {
                    x.Produces<GetCartResponse>(200);
                    x.Produces(401);
                    x.WithTags("Cart");
                });
            }

            public override async Task HandleAsync(CancellationToken c)
            {
                var userId = User.GetUserId();

                if (userId is null)
                {
                    await SendUnauthorizedAsync(c);
                    return;
                }

                var cart = await _cartRepository.GetCartByUserIdAsync(userId.Value);

                if (cart == null)
                {
                    cart = await _cartRepository.CreateEmptyCartAsync(userId.Value);
                }

                var response = new GetCartResponse
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(item => new CartItemResponse
                    {
                        ItemId = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }).ToList()
                };

                await SendOkAsync(response, c);
            }
        }
    }
}

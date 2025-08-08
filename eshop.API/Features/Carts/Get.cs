using eshop.Application.Contracts.Repositories;
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
            public string ProductName { get; set; } = "";
            public string ProductCoverUrl { get; set; } = "";
            public int ProductStock { get; set; }
            public decimal WeightInGrams { get; set; }
            public int Quantity { get; set; }
            public decimal DiscountPercentage { get; set; }
            public decimal BasePricePerUnit { get; set; }
            public decimal FinalPricePerUnit { get; set; }
            public decimal TotalPrice { get; set; }
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

                var cart = await _cartRepository.GetCartWithProductsByUserIdAsync(userId.Value);

                if (cart == null)
                {
                    cart = await _cartRepository.CreateEmptyCartAsync(userId.Value);
                }

                var response = new GetCartResponse
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(item =>
                    {
                        var basePrice = item.Product.Price;
                        var discountPct = item.Product.DiscountPercentage;
                        var discountedUnitPrice = Math.Round(basePrice * (1 - discountPct / 100m), 2);

                        return new CartItemResponse
                        {
                            ItemId = item.Id,
                            ProductId = item.ProductId,
                            ProductName = item.Product.Name,
                            ProductCoverUrl = item.Product.CoverPictureUrl,
                            ProductStock = item.Product.Stock,
                            WeightInGrams = item.Product.Weight,
                            Quantity = item.Quantity,
                            DiscountPercentage = discountPct,
                            BasePricePerUnit = basePrice,
                            FinalPricePerUnit = discountedUnitPrice,
                            TotalPrice = item.Quantity * discountedUnitPrice,
                        };
                    }).ToList()
                };

                await SendOkAsync(response, c);
            }
        }
    }
}

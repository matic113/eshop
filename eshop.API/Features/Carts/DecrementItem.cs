using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Application.Contracts.Services;
using eshop.Domain.Entities;
using eshop.Infrastructure.Extensions;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Carts
{
    public class DecrementItem
    {
        sealed class DecrementItemRequest
        {
            public Guid ItemId { get; set; }
            public int Quantity { get; set; } = 1;
        }

        sealed class DecrementItemValidator : Validator<DecrementItemRequest>
        {
            public DecrementItemValidator()
            {
                RuleFor(x => x.ItemId).NotEmpty().WithMessage("Product ID is required.");
                RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
            }
        }

        sealed class DecrementItemResponse
        {
            public string Message { get; set; } = string.Empty;
            public Guid? ItemId { get; set; }
            public Guid? ProductId { get; set; }
            public int Quantity { get; set; } = 0;
        }

        sealed class DecrementItemEndpoint : Endpoint<DecrementItemRequest, DecrementItemResponse>
        {
            private readonly ICartService _cartService;

            public DecrementItemEndpoint(ICartService cartService)
            {
                _cartService = cartService;
            }

            public override void Configure()
            {
                Post("/api/cart/items/decrement");
                Description(x =>
                {
                    x.Produces<DecrementItemResponse>(200);
                    x.Produces(401);
                    x.Produces(400);
                    x.WithTags("Cart");
                });
            }

            public override async Task HandleAsync(DecrementItemRequest r, CancellationToken c)
            {
                var userId = User.GetUserId();

                if (userId is null)
                {
                    await SendUnauthorizedAsync(c);
                    return;
                }

                var result = await _cartService.DecrementItemQuantityInUserCartAsync(userId.Value, r.ItemId, r.Quantity);

                if (result.IsError)
                {
                    await SendAsync(new DecrementItemResponse
                    {
                        Message = result.FirstError.Description
                    }, 400);

                    return;
                }

                var message = result.Value.WasRemoved ? "Item removed from cart."
                    : "Item quantity decremented successfully.";

                var response = new DecrementItemResponse
                {
                    Message = message,
                    ItemId = result.Value.CartItem!.Id,
                    ProductId = result.Value.CartItem.ProductId,
                    Quantity = result.Value.CartItem.Quantity
                };

                await SendOkAsync(response, c);
                return;
            }
        }
    }
}

using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Extensions;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Carts
{
    public class CreateItem
    {
        sealed class AddItemToCartRequest
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
        sealed class AddItemToCartValidator : Validator<AddItemToCartRequest>
        {
            public AddItemToCartValidator()
            {
                RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required.");
                RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
            }
        }
        sealed class AddItemToCartResponse
        {
            public string Message { get; set; } = string.Empty;
            public Guid? Id { get; set; }
            public Guid? ProductId { get; set; }
            public int Quantity { get; set; }
        }
        sealed class AddItemToCartEndpoint : Endpoint<AddItemToCartRequest, AddItemToCartResponse>
        {
            private readonly ICartService _cartService;

            public AddItemToCartEndpoint(ICartService cartService)
            {
                _cartService = cartService;
            }
            public override void Configure()
            {
                Post("/api/cart/items");
                Description(x =>
                {
                    x.Produces<AddItemToCartResponse>(200);
                    x.Produces(401);
                    x.Produces(400);
                    x.WithTags("Cart");
                });
            }

            public override async Task HandleAsync(AddItemToCartRequest r, CancellationToken c)
            {
                var userId = User.GetUserId();

                if (userId is null)
                {
                    await SendUnauthorizedAsync(c);
                    return;
                }

                var result = await _cartService.AddItemToUserCartAsync(userId.Value, r.ProductId, r.Quantity);

                if (result.IsError)
                {
                    AddError($"{result.FirstError.Code}: {result.FirstError.Description}");
                    await SendErrorsAsync(cancellation: c);
                    return;
                }

                var response = new AddItemToCartResponse
                {
                    Message = "Item added to cart successfully.",
                    Id = result.Value.Id,
                    ProductId = result.Value.ProductId,
                    Quantity = result.Value.Quantity
                };

                await SendOkAsync(response, c);
            }
        }
    }
}

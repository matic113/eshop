using System.Diagnostics.Eventing.Reader;
using eshop.Application.Contracts;
using eshop.Domain.Entities;
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
            public Guid Id { get; set; }
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
        sealed class AddItemToCartEndpoint : Endpoint<AddItemToCartRequest, AddItemToCartResponse>
        {
            private readonly ICartRepository _cartRepository;
            private readonly IGenericRepository<Product> _productRepository;
            private readonly IGenericRepository<CartItem> _cartItemRepository;

            public AddItemToCartEndpoint(ICartRepository cartRepository, IGenericRepository<Product> productRepository, IGenericRepository<CartItem> cartItemRepository)
            {
                _cartRepository = cartRepository;
                _productRepository = productRepository;
                _cartItemRepository = cartItemRepository;
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

                var cart = await _cartRepository.GetCartByUserIdAsync(userId.Value);

                if (cart == null)
                {
                    cart = await _cartRepository.CreateEmptyCartAsync(userId.Value);
                }

                var productExists = await _productRepository.GetByIdAsync(r.ProductId);

                if (productExists == null)
                {
                    AddError(x => x.ProductId, "Product does not exist.");
                    await SendErrorsAsync();
                    return;
                }

                var isItemInCart = cart.CartItems.Any(i => i.ProductId == r.ProductId);

                if (isItemInCart)
                {
                    AddError(x => x.ProductId, "Item already exists in the cart.");
                    await SendErrorsAsync();
                    return;
                }

                var newItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = r.ProductId,
                    Quantity = r.Quantity
                };

                // First save the cart item
                await _cartItemRepository.AddAsync(newItem);

                cart.CartItems.Add(newItem);
                await _cartRepository.UpdateAsync(cart);

                var response = new AddItemToCartResponse
                {
                    Message = "Item added to cart successfully.",
                    Id = newItem.Id,
                    ProductId = newItem.ProductId,
                    Quantity = newItem.Quantity
                };

                await SendOkAsync(response, c);
            }
        }
    }
}

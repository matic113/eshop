using eshop.Application.Contracts;
using eshop.Domain.Entities;
using eshop.Infrastructure.Extensions;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

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
            public Guid ItemId { get; set; }
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }

        sealed class DecrementItemEndpoint : Endpoint<DecrementItemRequest, DecrementItemResponse>
        {
            private readonly ICartRepository _cartRepository;
            private readonly IGenericRepository<CartItem> _cartItemRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DecrementItemEndpoint(ICartRepository cartRepository, IUnitOfWork unitOfWork, IGenericRepository<CartItem> cartItemRepository)
            {
                _cartRepository = cartRepository;
                _unitOfWork = unitOfWork;
                _cartItemRepository = cartItemRepository;
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

                var cart = await _cartRepository.GetCartByUserIdAsync(userId.Value);

                if (cart is null)
                {
                    await SendNotFoundAsync(c);
                    return;
                }

                var existingItem = cart.CartItems.FirstOrDefault(i => i.Id == r.ItemId);

                if (existingItem is null)
                {
                    await SendNotFoundAsync(c);
                    return;
                }

                if (existingItem.Quantity <= r.Quantity)
                {
                    bool result = await _cartRepository.RemoveItemFromCartAsync(userId.Value, existingItem.Id);
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
                else
                {
                    // Decrement the quantity of the existing item
                    existingItem.Quantity -= r.Quantity;
                    await _cartItemRepository.UpdateAsync(existingItem);
                    await _unitOfWork.SaveChangesAsync(c);

                    var response = new DecrementItemResponse
                    {
                        Message = "Item quantity decremented successfully.",
                        ItemId = existingItem.Id,
                        ProductId = existingItem.ProductId,
                        Quantity = existingItem.Quantity
                    };
                    await SendOkAsync(response, c);
                }
            }
        }
    }
}

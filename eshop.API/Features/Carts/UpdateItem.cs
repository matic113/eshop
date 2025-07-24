using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Infrastructure.Extensions;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace eshop.API.Features.Carts
{
    public class UpdateItem
    {
        sealed class UpdateItemRequest
        {
            [FromRoute]
            public Guid Id { get; set; }
            public int Quantity { get; set; }
        }
        sealed class UpdateItemRequestValidator : Validator<UpdateItemRequest>
        {
            public UpdateItemRequestValidator()
            {
                RuleFor(x => x.Id).NotEmpty().WithMessage("Item ID is required.");
                RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
            }
        }
        sealed class UpdateItemResponse
        {
            public string Message { get; set; } = String.Empty;
            public Guid Id { get; set; }
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }

        sealed class UpdateItemEndpoint : Endpoint<UpdateItemRequest, UpdateItemResponse>
        {
            private readonly ICartRepository _cartRepository;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateItemEndpoint(ICartRepository cartRepository, IUnitOfWork unitOfWork)
            {
                _cartRepository = cartRepository;
                _unitOfWork = unitOfWork;
            }

            public override void Configure()
            {
                Put("/api/cart/items/{Id}");
                Description(x =>
                {
                    x.Produces<UpdateItemResponse>(200);
                    x.Produces(401);
                    x.Produces(400);
                    x.WithTags("Cart");
                });
            }

            public override async Task HandleAsync(UpdateItemRequest r, CancellationToken c)
            {
                var userId = User.GetUserId();

                if (userId is null)
                {
                    await SendUnauthorizedAsync(c);
                    return;
                }

                var result = await _cartRepository.UpdateItemQuantityAsync(userId.Value, r.Id, r.Quantity);
                await _unitOfWork.SaveChangesAsync(c);

                if (result is null)
                {
                    AddError(x => x.Id, "Item not found in user cart.");
                    await SendErrorsAsync();
                    return;
                }

                var response = new UpdateItemResponse
                {
                    Message = "Item quantity updated successfully.",
                    Id = result.Id,
                    ProductId = result.ProductId,
                    Quantity = result.Quantity
                };

                await SendOkAsync(response, c);
            }
        }
    }
}

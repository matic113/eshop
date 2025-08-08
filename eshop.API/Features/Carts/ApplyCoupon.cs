using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Extensions;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Carts
{
    public class ApplyCoupon
    {
        sealed class ApplyCouponRequest
        {
            public string CouponCode { get; set; } = string.Empty;
        }
        sealed class ApplyCouponValidator : Validator<ApplyCouponRequest>
        {
            public ApplyCouponValidator()
            {
                RuleFor(x => x.CouponCode)
                    .NotEmpty().WithMessage("Coupon code is required.")
                    .MaximumLength(50).WithMessage("Coupon code can't be more that 50 characters");
            }
        }

        sealed class ApplyCouponResponse
        {
            public Guid CartId { get; set; }
            public decimal OriginalTotal { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal FinalTotal { get; set; }
            public CouponItem Coupon { get; set; } = null!;
        }

        sealed class CouponItem
        {
            public string Code { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public decimal Value { get; set; }
        }

        sealed class ApplyCouponEndpoint : Endpoint<ApplyCouponRequest, ApplyCouponResponse>
        {
            private readonly ICartService _cartService;

            public ApplyCouponEndpoint(ICartService cartService)
            {
                _cartService = cartService;
            }

            public override void Configure()
            {
                Post("/api/cart/apply-coupon");
                Description(x =>
                {
                    x.Produces<ApplyCouponResponse>(200);
                    x.Produces(400);
                    x.Produces(401);
                    x.WithTags("Cart");
                });
            }

            public override async Task HandleAsync(ApplyCouponRequest r, CancellationToken c)
            {
                var userId = User.GetUserId();

                if (userId is null)
                {
                    await SendUnauthorizedAsync();
                    return;
                }

                var result = await _cartService.ApplyCouponToCartAsync(userId.Value, r.CouponCode);

                if (result.IsError)
                {
                    var errorMessage = $"{result.FirstError.Code}: {result.FirstError.Description}";
                    AddError(errorMessage);
                    await SendErrorsAsync();
                    return;
                }

                var cartPriceDto = result.Value;
                var response = new ApplyCouponResponse
                {
                    CartId = cartPriceDto.CartId,
                    OriginalTotal = cartPriceDto.OriginalPrice,
                    DiscountAmount = cartPriceDto.DiscountAmount,
                    FinalTotal = cartPriceDto.FinalPrice,
                    Coupon = new CouponItem
                    {
                        Code = cartPriceDto.CouponCode ?? "",
                        Type = cartPriceDto.CouponType ?? "",
                        Value = cartPriceDto.CouponValue ?? 0
                    }
                };

                await SendOkAsync(response);
            }
        }
    }
}

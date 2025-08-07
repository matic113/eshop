using eshop.Application.Contracts.Repositories;
using eshop.Application.Contracts.Services;
using eshop.Domain.Entities;
using eshop.Domain.Enums;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Coupons
{
    public class Create
    {
        sealed class CreateCouponRequest
        {
            public string CouponCode { get; set; } = null!;
            public string CouponType { get; set; } = null!;
            public DateTime ExpirationDate { get; set; }
            public int UsageTimes { get; set; }
            public int TimesPerUser { get; set; }
            public decimal DiscountValue { get; set; }
            public decimal MaxDiscount { get; set; }
        }

        sealed class CreateCouponValidator : Validator<CreateCouponRequest>
        {
            public CreateCouponValidator()
            {
                RuleFor(x => x.CouponCode)
                    .NotEmpty()
                    .WithMessage("Coupon code is required.")
                    .MaximumLength(50)
                    .WithMessage("Coupon code can't be more that 50 characters.");
                RuleFor(x => x.CouponType)
                    .NotEmpty()
                    .WithMessage("Coupon type is required.");
                RuleFor(x => x.ExpirationDate)
                    .GreaterThan(DateTime.UtcNow)
                    .WithMessage("Expiration date must be in the future.");
                RuleFor(x => x.UsageTimes)
                    .GreaterThan(0)
                    .WithMessage("Usage times must be greater than zero.");
                RuleFor(x => x.TimesPerUser)
                    .GreaterThan(0)
                    .WithMessage("Times per user must be greater than zero.");
                RuleFor(x => x.DiscountValue)
                    .GreaterThan(0)
                    .WithMessage("Discount value must be greater than zero.");

                When(x => x.CouponType.ToLower() == CouponType.Percentage.ToString().ToLower(), () =>
                {
                    RuleFor(x => x.MaxDiscount)
                    .GreaterThan(0)
                    .WithMessage("Max discount must be greater than 0 for Percentage Discounts.");
                });
            }
        }

        sealed class CreateCouponResponse
        {
            public Guid Id { get; set; }
            public string CouponCode { get; set; } = null!;
            public string CouponType { get; set; } = null!;
            public DateTime ExpirationDate { get; set; }
            public int UsagesLeft { get; set; }
            public int TimesPerUser { get; set; }
            public decimal DiscountValue { get; set; }
            public decimal MaxDiscount { get; set; }
        }

        sealed class CreateCouponEndpoint : Endpoint<CreateCouponRequest, CreateCouponResponse>
        {
            private readonly ICouponService _couponService;

            public CreateCouponEndpoint(ICouponService couponService)
            {
                _couponService = couponService;
            }

            public override void Configure()
            {
                Post("/api/coupons");
                Description(x =>
                {
                    x.WithTags("Coupons");
                    x.Accepts<CreateCouponRequest>("application/json");
                    x.Produces<CreateCouponResponse>(201);
                });
            }

            public override async Task HandleAsync(CreateCouponRequest r, CancellationToken c)
            {
                var result = await _couponService.CreateCouponAsync(
                    r.CouponCode,
                    r.CouponType,
                    r.ExpirationDate,
                    r.UsageTimes,
                    r.TimesPerUser,
                    r.DiscountValue,
                    r.MaxDiscount);

                if (result.IsError)
                {
                    var errorMessage = $"{result.FirstError.Code}: {result.FirstError.Description}";
                    AddError(errorMessage);
                    await SendErrorsAsync();
                    return;
                }
                
                var couponDto = result.Value;
                var response = new CreateCouponResponse
                {
                    Id = couponDto.Id,
                    CouponCode = couponDto.CouponCode,
                    CouponType = couponDto.CouponType,
                    ExpirationDate = couponDto.ExpiresAt,
                    UsagesLeft = couponDto.UsagesLeft,
                    TimesPerUser = couponDto.TimesPerUser,
                    DiscountValue = couponDto.DiscountValue,
                    MaxDiscount = couponDto.MaxDiscount
                };

                await SendOkAsync(response);
            }
        }
    }
}

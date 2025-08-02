using ErrorOr;
using eshop.Application.Contracts.Services;
using eshop.Application.Dtos;
using eshop.Domain.Enums;
using eshop.Infrastructure.Extensions;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Orders
{
    public class Checkout
    {
        sealed class OrderCheckoutRequest
        {
            public Guid ShippingAddressId { get; set; }
            public string PaymentMethod { get; set; }
        }

        sealed class OrderCheckoutValidator : Validator<OrderCheckoutRequest>
        {
            public OrderCheckoutValidator()
            {
                RuleFor(x => x.ShippingAddressId)
                    .NotEmpty().WithMessage("Shipping address ID is required.");
                RuleFor(x => x.PaymentMethod)
                    .NotEmpty().WithMessage("Payment method is required.")
                    .Must(pm => pm == PaymentMethod.CashOnDelivery.ToString() || pm == PaymentMethod.Paymob.ToString())
                    .WithMessage("Invalid payment method.");
            }
        }
        sealed class OrderCheckoutResponse
        {
            public required string Message { get; set; }

            // Can be empty in case of CashOnDeliveryPayment
            public string UnifiedCheckoutUrl { get; set; } = "";
            public string PaymentClientSecret { get; set; } = "";
        }

        sealed class OrderCheckoutEndpoint : Endpoint<OrderCheckoutRequest, OrderCheckoutResponse>
        {
            private readonly IOrderService _orderService;

            public OrderCheckoutEndpoint(IOrderService orderService)
            {
                _orderService = orderService;
            }

            public override void Configure()
            {
                Post("/api/orders/checkout");
                Description(x =>
                {
                    x.WithTags("Orders");
                });
            }

            public override async Task HandleAsync(OrderCheckoutRequest r, CancellationToken c)
            {
                var userId = User.GetUserId();

                if (userId is null)
                {
                    await SendUnauthorizedAsync();
                    return;
                }

                ErrorOr<OrderCheckoutDto> checkoutDto = await _orderService.CheckoutAsync(userId.Value, r.ShippingAddressId, r.PaymentMethod);

                if (checkoutDto.IsError)
                {
                    AddError($"{checkoutDto.FirstError.Code}: {checkoutDto.FirstError.Description}");
                    await SendErrorsAsync(cancellation: c);
                    return;
                }

                string message = r.PaymentMethod == PaymentMethod.Paymob.ToString()
                    ? "Order placed successfully. Please complete the payment to confirm your order."
                    : "Order placed successfully.";

                var response = new OrderCheckoutResponse
                {
                    Message = message,
                    UnifiedCheckoutUrl = checkoutDto.Value.UnifiedCheckoutUrl,
                    PaymentClientSecret = checkoutDto.Value.PaymentClientSecret
                };

                await SendOkAsync(response);
            }
        }
    }
}

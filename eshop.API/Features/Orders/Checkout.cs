using ErrorOr;
using eshop.Application.Contracts;
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
            public string PaymentKey { get; set; } = "";
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
                    var error = checkoutDto.FirstError;
                    await SendAsync(new()
                    {
                        Message = error.Description ?? "Something went wrong while placing the order."
                    }, StatusCodes.Status400BadRequest);
                    return;
                }

                string message = r.PaymentMethod == PaymentMethod.Paymob.ToString()
                    ? "Order placed successfully. Please complete the payment to confirm your order."
                    : "Order placed successfully.";

                var response = new OrderCheckoutResponse
                {
                    Message = message,
                    PaymentKey = checkoutDto.Value.PaymentKey,
                    PaymentClientSecret = checkoutDto.Value.PaymentClientSecret
                };

                await SendOkAsync(response);
            }
        }
    }
}

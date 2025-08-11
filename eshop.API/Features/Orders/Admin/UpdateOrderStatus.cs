using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Authentication;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Orders.Admin
{
    public class UpdateOrderStatus
    {
        sealed class UpdateOrderStatusRequest
        {
            public Guid OrderId { get; set; }
            public string Status { get; set; } = string.Empty;
            public string Notes { get; set; } = string.Empty;
        }
        sealed class UpdateOrderStatusRequestValidator : Validator<UpdateOrderStatusRequest>
        {
            public UpdateOrderStatusRequestValidator()
            {
                RuleFor(x => x.OrderId)
                    .NotEmpty().WithMessage("Order ID is required.");
                RuleFor(x => x.Status)
                    .NotEmpty().WithMessage("Status is required.");
                RuleFor(x => x.Notes)
                    .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
            }
        }
        sealed class UpdateOrderStatusEndpoint : Endpoint<UpdateOrderStatusRequest>
        {
            private readonly IOrderService _orderService;

            public UpdateOrderStatusEndpoint(IOrderService orderService)
            {
                _orderService = orderService;
            }

            public override void Configure()
            {
                Put("/api/orders/admin/order-status");
                Roles(ApplicationRoles.Admin);
                Description(x => x
                    .WithTags("Orders")
                    .WithDescription("Update order status for admin.")
                    .Produces(200)
                    .ProducesProblem(400)
                    .ProducesProblem(401)
                    .ProducesProblem(403));
            }

            public override async Task HandleAsync(UpdateOrderStatusRequest r, CancellationToken c)
            {
                var result = await _orderService.UpdateOrderStatusAsync(r.OrderId, r.Status, r.Notes);

                if (result.IsError)
                {
                    var errorMessage = $"{result.FirstError.Code}: {result.FirstError.Description}";
                    AddError(errorMessage);
                    await SendErrorsAsync();
                    return;
                }

                await SendOkAsync(new { Message = "Order status updated successfully." }, c);
            }
        }
    }
}

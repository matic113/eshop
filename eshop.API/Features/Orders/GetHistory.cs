using eshop.Application.Contracts.Services;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace eshop.API.Features.Orders
{
    public class GetHistory
    {
        sealed class GetOrdersHistoryRequest
        {
            [FromRoute]
            public Guid OrderId { get; set; }
        }
        sealed class GetOrdersHistoryValidator : Validator<GetOrdersHistoryRequest>
        {
            public GetOrdersHistoryValidator()
            {
                RuleFor(x => x.OrderId)
                    .NotEmpty().WithMessage("Order ID is required.");
            }
        }
        sealed class GetOrdersHistoryResponse
        {
            public required string Message { get; set; }
            public Guid? OrderId { get; set; }
            public string? OrderCode { get; set; }
            public List<OrderHistorySingleEntry> History { get; set; } = new List<OrderHistorySingleEntry>();
        }
        sealed class OrderHistorySingleEntry
        {
            public required string OrderStatus { get; set; }
            public DateTime ChangeDate { get; set; }
            public string Notes { get; set; } = string.Empty;
        }

        sealed class GetOrdersHistoryEndpoint : Endpoint<GetOrdersHistoryRequest, GetOrdersHistoryResponse>
        {
            private readonly IOrderService _orderService;

            public GetOrdersHistoryEndpoint(IOrderService orderService)
            {
                _orderService = orderService;
            }

            public override void Configure()
            {
                Get("/api/orders/history/{orderId}");
                Description(x =>
                {
                    x.WithTags("Orders");
                });
            }

            public override async Task HandleAsync(GetOrdersHistoryRequest r, CancellationToken c)
            {
                var result = await _orderService.GetOrdersHistoryAsync(r.OrderId);

                if (string.IsNullOrEmpty(result.OrderCode))
                {
                    var badResponse = new GetOrdersHistoryResponse
                    {
                        Message = "Order not found or history not available.",
                        OrderId = r.OrderId
                    };

                    await SendAsync(badResponse, StatusCodes.Status400BadRequest);
                    return;
                }

                var response = new GetOrdersHistoryResponse
                {
                    Message = "Order history retrieved successfully.",
                    OrderId = r.OrderId,
                    // Take the last 6 characters of the OrderCode for display
                    OrderCode = result.OrderCode[^6..],
                    History = result.OrderStatusHistories
                        .Select(h => new OrderHistorySingleEntry
                        {
                            OrderStatus = h.OrderStatus.ToString(),
                            ChangeDate = h.ChangeDate,
                            Notes = h.Notes
                        }).ToList()
                };

                await SendOkAsync(response);
                return;
            }
        }
    }
}

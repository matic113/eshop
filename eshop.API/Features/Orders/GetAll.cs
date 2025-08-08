using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Extensions;
using FastEndpoints;

namespace eshop.API.Features.Orders
{
    public class GetAll
    {
        sealed class GetAllOrdersRequest
        {
            public int Limit { get; set; } = 10;
        }

        sealed class GetAllOrdersResponse
        {
            public required string Message { get; set; }
            public List<GetOrderEntry> Orders { get; set; } = new List<GetOrderEntry>();
        }
        sealed class GetOrderEntry
        {
            public required Guid OrderId { get; set; }
            public required string OrderCode { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public required string Status { get; set; }
            public required decimal TotalPrice { get; set; }
            public required string PaymentMethod { get; set; }
        }

        sealed class GetAllOrdersEndpoint : Endpoint<GetAllOrdersRequest, GetAllOrdersResponse>
        {
            private readonly IOrderService _orderService;

            public GetAllOrdersEndpoint(IOrderService orderService)
            {
                _orderService = orderService;
            }

            public override void Configure()
            {
                Get("/api/orders");
                Description(x =>
                {
                    x.WithTags("Orders");
                });
            }

            public override async Task HandleAsync(GetAllOrdersRequest r, CancellationToken c)
            {
                var userId = User.GetUserId();

                if (userId is null)
                {
                    await SendUnauthorizedAsync();
                    return;
                }

                var orders = await _orderService.GetOrdersByUserIdAsync(userId.Value, r.Limit);

                if (orders == null || !orders.Any())
                {
                    var badResponse = new GetAllOrdersResponse
                    {
                        Message = "Orders.NotFound: No orders found."
                    };
                    await SendOkAsync(badResponse);
                    return;
                }

                var response = new GetAllOrdersResponse
                {
                    Message = "Orders retrieved successfully.",
                    Orders = orders.Select(o => new GetOrderEntry
                    {
                        OrderId = o.Id,
                        // Take the last 6 characters of the OrderNumber for display
                        OrderCode = o.OrderNumber[^6..],
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt,
                        Status = o.Status.ToString(),
                        TotalPrice = o.TotalPrice,
                        PaymentMethod = o.PaymentMethod.ToString()
                    }).ToList()
                };

                await SendOkAsync(response);
            }
        }
    }
}

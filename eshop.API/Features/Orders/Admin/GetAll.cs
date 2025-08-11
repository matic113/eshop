using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Authentication;
using eShop.Application.Dtos.Orders;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Orders.Admin
{
    public partial class GetAll
    {
        sealed class GetAllOrdersForAdminRequest
        {
            public string Period { get; set; } = string.Empty;
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }

        sealed class GetAllOrdersForAdminRequestValidator : Validator<GetAllOrdersForAdminRequest>
        {
            public GetAllOrdersForAdminRequestValidator()
            {
                RuleFor(x => x.Period)
                    .Must(p => new[] { "1d", "3d", "7d", "30d" }.Contains(p))
                    .WithMessage("Period can be one these [\"1d\", \"3d\", \"7d\", \"30d\"]");
            }
        }
        sealed class GetAllOrdersForAdminResponse
        {
            public string Period { get; set; } = null!;
            public PagedList<GetOrderDto> Orders { get; set; } = null!;
        }

        sealed class GetAllOrdersForAdminEndpoint : Endpoint<GetAllOrdersForAdminRequest, GetAllOrdersForAdminResponse>
        {
            private readonly IOrderService _orderService;

            public GetAllOrdersForAdminEndpoint(IOrderService orderService)
            {
                _orderService = orderService;
            }

            public override void Configure()
            {
                Get("/api/orders/admin");
                Roles(ApplicationRoles.Admin);
            }

            public override async Task HandleAsync(GetAllOrdersForAdminRequest r, CancellationToken c)
            {
                var orders = await _orderService.GetAllOrdersAsync(r.Period, r.Page, r.PageSize);

                var response = new GetAllOrdersForAdminResponse
                {
                    Period = r.Period,
                    Orders = orders
                };

                await SendOkAsync(response, c);
            }
        }
    }
}

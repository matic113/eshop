using eshop.Application.Contracts.Repositories;
using eshop.Application.Dtos;
using FastEndpoints;

namespace eshop.API.Features.Coupons
{
    public class ListAll
    {
        sealed class GetAllCouponsResponse
        {
            public List<CouponDto> Coupons { get; set; } = new List<CouponDto>();
        }
        sealed class GetAllCouponsEndpoint : EndpointWithoutRequest<GetAllCouponsResponse>
        {
            private readonly ICouponRepository _couponRepository;

            public GetAllCouponsEndpoint(ICouponRepository couponRepository)
            {
                _couponRepository = couponRepository;
            }

            public override void Configure()
            {
                Get("/api/coupons");
                Description(x =>
                {
                    x.WithTags("Coupons");
                });
            }

            public override async Task HandleAsync(CancellationToken c)
            {
                var coupons = await _couponRepository.GetAllCouponsAsync();
                var response = new GetAllCouponsResponse
                {
                    Coupons = coupons
                };
                await SendOkAsync(response);
            }
        }
    }
}

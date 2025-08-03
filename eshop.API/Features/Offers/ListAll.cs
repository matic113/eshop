using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using eshop.Infrastructure.Repositories;
using FastEndpoints;

namespace eshop.API.Features.Offers
{
    public class ListAll
    {
        sealed class GetAllOffersRequest
        {
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 6;
        }
        sealed class GetAllOffersResponse
        {
            public PagedList<Offer> Offers { get; set; } = null!;
        }
        sealed class GetAllOffersEndpoint : Endpoint<GetAllOffersRequest, GetAllOffersResponse>
        {
            private readonly IOfferRepository _offersRepository;
            public GetAllOffersEndpoint(IOfferRepository offersRepository)
            {
                _offersRepository = offersRepository;
            }

            public override void Configure()
            {
                Get("/api/offers/");
                AllowAnonymous();
                Description(x => x
                    .WithTags("Offers")
                    .WithDescription("Get all offers")
                    .WithName("GetAllOffers")
                    .Produces<GetAllOffersResponse>()
                    .ProducesValidationProblem());
            }

            public override async Task HandleAsync(GetAllOffersRequest r, CancellationToken c)
            {
                var offers = await _offersRepository.GetAllOffers(r.Page, r.PageSize);

                await SendOkAsync(new GetAllOffersResponse
                {
                    Offers = offers
                });
            }
        }
    }
}

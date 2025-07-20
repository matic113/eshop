using eshop.Application.Contracts;
using eshop.Infrastructure.Extensions;
using FastEndpoints;

namespace eshop.API.Features.Addresses
{
    public class Get
    {
        sealed class GetAddressResponse
        {
            public Guid Id { get; set; }
            public string State { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Street { get; set; } = string.Empty;
            public string Apartment { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Notes { get; set; } = string.Empty;
        }

        sealed class GetAddressEndpoint : EndpointWithoutRequest<GetAddressResponse>
        {
            private readonly IAddressRepository _addressRepository;

            public GetAddressEndpoint(IAddressRepository addressRepository)
            {
                _addressRepository = addressRepository;
            }

            public override void Configure()
            {
                Get("/api/addresses/{Id}");
                Description(x => x
                    .WithName("GetAddress")
                    .WithTags("Addresses")
                    .Produces<GetAddressResponse>(200)
                    .ProducesProblem(401)
                    .ProducesProblem(404));
            }

            public override async Task HandleAsync(CancellationToken c)
            {
                var addressId = Route<Guid>("Id");

                if (addressId == Guid.Empty)
                {
                    await SendNotFoundAsync(c);
                    return;
                }

                var address = await _addressRepository.GetByIdAsync(addressId);

                if (address is null)
                {
                    await SendNotFoundAsync(c);
                    return;
                }

                var userId = User.GetUserId();

                if (userId is null || address.UserId != userId.Value)
                {
                    await SendUnauthorizedAsync(c);
                    return;
                }

                var response = new GetAddressResponse
                {
                    Id = address.Id,
                    State = address.State,
                    City = address.City,
                    Street = address.Street,
                    Apartment = address.Apartment,
                    PhoneNumber = address.PhoneNumber,
                    Notes = address.Notes
                };

                await SendOkAsync(response, c);
            }
        }
    }
}

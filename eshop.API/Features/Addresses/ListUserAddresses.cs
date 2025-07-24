using eshop.Application.Contracts.Repositories;
using eshop.Infrastructure.Extensions;
using FastEndpoints;

namespace eshop.API.Features.Addresses
{
    public class ListUserAddresses
    {
        sealed class ListAllAddressesResponse
        {
            public Guid Id { get; set; }
            public string State { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Street { get; set; } = string.Empty;
            public string Apartment { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Notes { get; set; } = string.Empty;
        }
        sealed class ListAllAddressesEndpoint : EndpointWithoutRequest<IEnumerable<ListAllAddressesResponse>>
        {
            private readonly IAddressRepository _addressRepository;

            public ListAllAddressesEndpoint(IAddressRepository addressRepository)
            {
                _addressRepository = addressRepository;
            }

            public override void Configure()
            {
                Get("/api/addresses");
                Description(x => x
                    .WithName("ListUserAddresses")
                    .WithTags("Addresses")
                    .Produces<IEnumerable<ListAllAddressesResponse>>(200)
                    .ProducesProblem(401));
            }

            public override async Task HandleAsync(CancellationToken c)
            {
                var userId = User.GetUserId();

                if (userId is null)
                {
                    await SendUnauthorizedAsync();
                    return;
                }

                var addresses = await _addressRepository.GetUserAddressesAsync(userId.Value);

                var response = addresses.Select(a => new ListAllAddressesResponse
                {
                    Id = a.Id,
                    State = a.State,
                    City = a.City,
                    Street = a.Street,
                    Apartment = a.Apartment,
                    PhoneNumber = a.PhoneNumber,
                    Notes = a.Notes
                });

                await SendOkAsync(response, c);
            }
        }
    }
}

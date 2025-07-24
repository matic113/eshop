using eshop.Application.Contracts;
using eshop.Infrastructure.Extensions;
using FastEndpoints;

namespace eshop.API.Features.Addresses
{
    public class Delete
    {
        sealed class DeleteAddressEndpoint : EndpointWithoutRequest
        {
            private readonly IAddressRepository _addressRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteAddressEndpoint(IAddressRepository addressRepository, IUnitOfWork unitOfWork)
            {
                _addressRepository = addressRepository;
                _unitOfWork = unitOfWork;
            }

            public override void Configure()
            {
                Delete("/api/addresses/{Id}");
                Description(x => x
                    .WithName("DeleteAddress")
                    .WithTags("Addresses")
                    .Produces(200)
                    .ProducesProblem(404)
                    .ProducesProblem(401));
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

                // make sure the address belongs to the current user
                var userId = User.GetUserId();

                if (userId is null || address.UserId != userId.Value)
                {
                    await SendUnauthorizedAsync(c);
                    return;
                }

                var result = await _addressRepository.DeleteAsync(addressId);
                await _unitOfWork.SaveChangesAsync(c);

                if (!result)
                {
                    AddError("Failed to delete the address.");
                    await SendErrorsAsync(cancellation: c);
                    return;
                }

                await SendOkAsync(c);
                return;
            }
        }
    }
}

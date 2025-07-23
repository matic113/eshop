using eshop.Application.Contracts;
using eshop.Infrastructure.Extensions;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace eshop.API.Features.Addresses
{
    public class Update
    {
        sealed class UpdateAddressRequest
        {
            [FromRoute]
            public Guid Id { get; set; }
            public string State { get; set; }
            public string City { get; set; }
            public string Street { get; set; }
            public string Apartment { get; set; }
            public string PhoneNumber { get; set; }
            public string Notes { get; set; }
        }
        sealed class UpdateAddressValidator : Validator<UpdateAddressRequest>
        {
            public UpdateAddressValidator()
            {
                RuleFor(x => x.State)
                    .MaximumLength(20).WithMessage("State can't be more than 20 characters");
                RuleFor(x => x.City)
                    .MaximumLength(30).WithMessage("City can't be more than 20 characters");
                RuleFor(x => x.Street)
                    .MaximumLength(50).WithMessage("Street can't be more than 20 characters");
                RuleFor(x => x.Apartment)
                    .MaximumLength(50).WithMessage("Apartment can't be more than 20 characters");
                RuleFor(x => x.PhoneNumber)
                    .Matches(@"^01[0125]\d{8}$").WithMessage("Invalid phone number format.");
                RuleFor(x => x.Notes)
                    .MaximumLength(400).WithMessage("Notes can't be more than 400 characters");
            }
        }

        sealed class UpdateAddressResponse
        {
            public Guid Id { get; set; }
            public string State { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Street { get; set; } = string.Empty;
            public string Apartment { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Notes { get; set; } = string.Empty;
        }

        sealed class UpdateAddressEndpoint : Endpoint<UpdateAddressRequest, UpdateAddressResponse>
        {
            private readonly IAddressRepository _addressRepository;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateAddressEndpoint(IAddressRepository addressRepository, IUnitOfWork unitOfWork)
            {
                _addressRepository = addressRepository;
                _unitOfWork = unitOfWork;
            }

            public override void Configure()
            {
                Put("/api/addresses/{Id}");
                Description(x => x
                    .WithName("UpdateAddress")
                    .WithTags("Addresses")
                    .Produces<UpdateAddressResponse>(200)
                    .ProducesProblem(401)
                    .ProducesProblem(404));
            }

            public override async Task HandleAsync(UpdateAddressRequest r, CancellationToken c)
            {
                var address = await _addressRepository.GetByIdAsync(r.Id);

                if (address is null)
                {
                    await SendNotFoundAsync(cancellation: c);
                    return;
                }

                var userId = User.GetUserId();

                if (userId is null || address.UserId != userId.Value)
                {
                    await SendUnauthorizedAsync(cancellation: c);
                    return;
                }

                if (r.State is not null) address.State = r.State;
                if (r.City is not null) address.City = r.City;
                if (r.Street is not null) address.Street = r.Street;
                if (r.Apartment is not null) address.Apartment = r.Apartment;
                if (r.PhoneNumber is not null) address.PhoneNumber = r.PhoneNumber;
                if (r.Notes is not null) address.Notes = r.Notes;

                await _addressRepository.UpdateAsync(address);
                await _unitOfWork.SaveChangesAsync(c);

                var response = new UpdateAddressResponse
                {
                    Id = address.Id,
                    State = address.State,
                    City = address.City,
                    Street = address.Street,
                    Apartment = address.Apartment,
                    PhoneNumber = address.PhoneNumber,
                    Notes = address.Notes
                };

                await SendOkAsync(response, cancellation: c);
                return;
            }
        }
    }
}

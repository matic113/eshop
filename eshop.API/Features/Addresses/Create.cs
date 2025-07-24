using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using eshop.Infrastructure.Extensions;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Addresses
{
    public class Create
    {
        sealed class CreateAddressRequest
        {
            public string State { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Street { get; set; } = string.Empty;
            public string Apartment { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Notes { get; set; } = string.Empty;
        }
        sealed class CreateAddressValidator : Validator<CreateAddressRequest>
        {
            public CreateAddressValidator()
            {
                RuleFor(x => x.State)
                    .NotEmpty().WithMessage("State is required.")
                    .MaximumLength(20).WithMessage("State can't be more that 20 characters");
                RuleFor(x => x.City)
                    .NotEmpty().WithMessage("City is required.")
                    .MaximumLength(30).WithMessage("City can't be more that 20 characters");
                RuleFor(x => x.Street)
                    .NotEmpty().WithMessage("Street is required.")
                    .MaximumLength(50).WithMessage("Street can't be more that 20 characters");
                RuleFor(x => x.Apartment)
                    .NotEmpty().WithMessage("Apartment is required.")
                    .MaximumLength(50).WithMessage("Apartment can't be more that 20 characters");
                RuleFor(x => x.PhoneNumber)
                    .NotEmpty().WithMessage("Phone number is required.")
                    .Matches(@"^01[0125]\d{8}$").WithMessage("Invalid phone number format.");
                RuleFor(x => x.Notes)
                    .MaximumLength(400).WithMessage("Notes can't be more that 400 characters");
            }
        }
        sealed class CreateAddressResponse
        {
            public Guid Id { get; set; }
            public string State { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Street { get; set; } = string.Empty;
            public string Apartment { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Notes { get; set; } = string.Empty;
        }

        sealed class CreateAddressEndpoint : Endpoint<CreateAddressRequest, CreateAddressResponse>
        {
            private readonly IAddressRepository _addressRepository;
            private readonly IUnitOfWork _unitOfWork;

            public CreateAddressEndpoint(IAddressRepository addressRepository, IUnitOfWork unitOfWork)
            {
                _addressRepository = addressRepository;
                _unitOfWork = unitOfWork;
            }

            public override void Configure()
            {
                Post("/api/addresses");
                Description(x => x
                    .WithName("CreateAddress")
                    .WithTags("Addresses")
                    .Produces<CreateAddressResponse>(200)
                    .ProducesValidationProblem(400));
            }

            public override async Task HandleAsync(CreateAddressRequest r, CancellationToken c)
            {
                var userId = User.GetUserId();

                if (userId is null)
                {
                    await SendUnauthorizedAsync();
                    return;
                }

                var address = new Address
                {
                    Id = Guid.NewGuid(),
                    UserId = userId.Value,
                    State = r.State,
                    City = r.City,
                    Street = r.Street,
                    Apartment = r.Apartment,
                    PhoneNumber = r.PhoneNumber,
                    Notes = r.Notes
                };

                await _addressRepository.AddAsync(address);
                await _unitOfWork.SaveChangesAsync(c);

                var response = new CreateAddressResponse
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

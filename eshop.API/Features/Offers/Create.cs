using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Offers
{
    public class Create
    {
        sealed class CreateOfferRequest
        {
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string OfferCoverUrl { get; set; } = string.Empty;
        }

        sealed class CreateOfferRequestValidator : Validator<CreateOfferRequest>
        {
            public CreateOfferRequestValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Offer name is required.")
                    .MaximumLength(100).WithMessage("Offer name cannot exceed 100 characters.");
                RuleFor(x => x.Description)
                    .MaximumLength(150).WithMessage("Description cannot exceed 150 characters.");
                RuleFor(x => x.OfferCoverUrl)
                    .NotEmpty().WithMessage("Offer cover URL is required.")
                    .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    .WithMessage("Cover picture URL must be a valid absolute URL.");
            }
        }
        sealed class CreateOfferResponse
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? OfferCoverPicture { get; set; }
        }

        sealed class CreateOfferEndpoint : Endpoint<CreateOfferRequest, CreateOfferResponse>
        {
            private readonly IGenericRepository<Offer> _offersRepository;
            private readonly IUnitOfWork _unitOfWork;

            public CreateOfferEndpoint(IGenericRepository<Offer> offersRepository, IUnitOfWork unitOfWork)
            {
                _offersRepository = offersRepository;
                _unitOfWork = unitOfWork;
            }

            public override void Configure()
            {
                Post("/api/offers/");
            }

            public override async Task HandleAsync(CreateOfferRequest r, CancellationToken c)
            {
                var offer = new Offer
                {
                    Id = Guid.NewGuid(),
                    Name = r.Name,
                    Description = r.Description ?? string.Empty,
                    CoverUrl = r.OfferCoverUrl
                };

                await _offersRepository.AddAsync(offer);
                await _unitOfWork.SaveChangesAsync(c);

                var response = new CreateOfferResponse
                {
                    Id = offer.Id,
                    Name = offer.Name,
                    Description = offer.Description,
                    OfferCoverPicture = offer.CoverUrl
                };
                await SendOkAsync(response);
            }
        }
    }
}

using eshop.Application.Contracts.Services;
using eshop.Application.Dtos;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Products
{
    sealed class Update
    {
        sealed class UpdateProductEndpoint : Endpoint<UpdateProductRequest, UpdateProductResponse>
        {
            private readonly IProductService _productService;

            public UpdateProductEndpoint(IProductService productService)
            {
                _productService = productService;
            }

            public override void Configure()
            {
                Put("/api/products/{Id}");
                AllowAnonymous();
                Description(x => x
                .WithTags("Products"));
            }

            public override async Task HandleAsync(UpdateProductRequest r, CancellationToken c)
            {
                var productId = Route<Guid>("Id");

                var result = await _productService.UpdateProductAsync(productId, r);

                if (result is null)
                {
                    await SendNotFoundAsync(c);
                    return;
                }

                var response = new UpdateProductResponse
                {
                    Id = result.Id,
                    SellerId = result.SellerId,
                    ProductCode = result.ProductCode,
                    Name = result.Name,
                    NameArabic = result.ArabicName,
                    Description = result.Description,
                    DescriptionArabic = result.ArabicDescription,
                    CoverPictureUrl = result.CoverPictureUrl,
                    Price = result.Price,
                    Stock = result.Stock,
                    Weight = result.Weight,
                    Color = result.Color,
                    DiscountPercentage = result.DiscountPercentage,
                    Categories = result.Categories,
                    ProductPictures = result.ProductPictures
                };

                await SendOkAsync(response);
            }
        }
        sealed class UpdateProductResponse
        {
            public Guid Id { get; set; }
            public Guid SellerId { get; set; }
            public string ProductCode { get; set; } = null!;
            public string Name { get; set; } = null!;
            public string NameArabic { get; set; } = null!;
            public string Description { get; set; } = null!;
            public string DescriptionArabic { get; set; } = null!;
            public string CoverPictureUrl { get; set; } = null!;
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public decimal Weight { get; set; }
            public string Color { get; set; } = null!;
            public short DiscountPercentage { get; set; } = 0;
            public List<string> Categories { get; set; } = new List<string>();
            public List<string> ProductPictures { get; set; } = new List<string>();
        }
        sealed class UpdateProductValidator : Validator<UpdateProductRequest>
        {
            public UpdateProductValidator()
            {
                RuleFor(x => x.Name)
                    .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.")
                    .When(x => x.Name is not null);

                RuleFor(x => x.NameArabic)
                    .MaximumLength(100).WithMessage("Product arabic name cannot exceed 100 characters.")
                    .When(x => x.NameArabic is not null);

                RuleFor(x => x.Description)
                    .MaximumLength(1000).WithMessage("Product description cannot exceed 1000 characters.")
                    .When(x => x.Description is not null);

                RuleFor(x => x.DescriptionArabic)
                    .MaximumLength(1000).WithMessage("Product arabic description cannot exceed 1000 characters.")
                    .When(x => x.DescriptionArabic is not null);

                RuleFor(x => x.CoverPictureUrl)
                    .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    .WithMessage("Cover picture URL must be a valid absolute URL.")
                    .When(x => x.CoverPictureUrl is not null);

                RuleFor(x => x.Price)
                    .GreaterThan(0).WithMessage("Price must be greater than zero.")
                    .When(x => x.Price.HasValue);

                RuleFor(x => x.Stock)
                    .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.")
                    .When(x => x.Stock.HasValue);

                RuleFor(x => x.Weight)
                    .GreaterThanOrEqualTo(0).WithMessage("Weight cannot be negative.")
                    .When(x => x.Weight.HasValue);

                RuleFor(x => x.Color)
                    .MaximumLength(20).WithMessage("Color cannot exceed 20 characters.")
                    .When(x => x.Color is not null);

                RuleFor(x => x.DiscountPercentage)
                    .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.")
                    .When(x => x.DiscountPercentage.HasValue);
            }
        }
    }
}

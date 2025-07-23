using eshop.Application.Contracts;
using eshop.Domain.Entities;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Products
{
    sealed class Update
    {
        sealed class UpdateProductEndpoint : Endpoint<UpdateProductRequest, UpdateProductResponse>
        {
            private readonly IGenericRepository<Product> _productRepository;
            private readonly IGenericRepository<Category> _categoryRepository;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateProductEndpoint(IGenericRepository<Product> productRepository, IGenericRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
            {
                _productRepository = productRepository;
                _categoryRepository = categoryRepository;
                _unitOfWork = unitOfWork;
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
                var product = await _productRepository.GetByIdAsync(productId);

                if (product is null)
                {
                    AddError("Product not found.");
                    await SendErrorsAsync();
                    return;
                }

                if (r.Name is not null) product.Name = r.Name;
                if (r.Description is not null) product.Description = r.Description;
                if (r.CoverPictureUrl is not null) product.CoverPictureUrl = r.CoverPictureUrl;
                if (r.Price.HasValue) product.Price = r.Price.Value;
                if (r.Stock.HasValue) product.Stock = r.Stock.Value;
                if (r.Weight.HasValue) product.Weight = r.Weight.Value;
                if (r.Color is not null) product.Color = r.Color;
                if (r.DiscountPercentage.HasValue) product.DiscountPercentage = (short)r.DiscountPercentage.Value;

                if (r.CategoryIds is not null && r.CategoryIds.Count != 0)
                {
                    // Clear existing categories and add new ones
                    product.Categories.Clear();

                    var existingCategories = await _categoryRepository.GetAllAsync();
                    foreach (var categoryId in r.CategoryIds)
                    {
                        var category = existingCategories.FirstOrDefault(c => c.Id == categoryId);
                        if (category is not null)
                        {
                            product.Categories.Add(category);
                        }
                        else
                        {
                            AddError($"Category with ID {categoryId} does not exist.");
                            await SendErrorsAsync();
                            return;
                        }
                    }
                }

                // Add new pictures
                if (r.ProductPictureUrls is not null)
                {
                    product.ProductPictures.Clear();

                    foreach (var pictureUrl in r.ProductPictureUrls)
                    {
                        if (string.IsNullOrWhiteSpace(pictureUrl))
                        {
                            continue; // Skip empty picture URLs
                        }
                        product.ProductPictures.Add(new ProductPicture
                        {
                            Id = Guid.NewGuid(),
                            ProductId = product.Id,
                            PictureUrl = pictureUrl
                        });
                    }
                }

                await _productRepository.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync(c);

                var response = new UpdateProductResponse
                {
                    Id = product.Id,
                    SellerId = product.SellerId,
                    ProductCode = product.ProductCode,
                    Name = product.Name,
                    Description = product.Description,
                    CoverPictureUrl = product.CoverPictureUrl,
                    Price = product.Price,
                    Stock = product.Stock,
                    Weight = product.Weight,
                    Color = product.Color,
                    DiscountPercentage = product.DiscountPercentage,
                    Categories = product.Categories
                                .Select(c => new CategoryResponse { Id = c.Id, Name = c.Name }).ToList(),
                    Pictures = product.ProductPictures
                                .Select(p => new ProductPictureResponse { Id = p.Id, PictureUrl = p.PictureUrl }).ToList()
                };

                await SendOkAsync(response, c);
            }
        }
        sealed class UpdateProductRequest
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? CoverPictureUrl { get; set; }
            public decimal? Price { get; set; }
            public int? Stock { get; set; }
            public decimal? Weight { get; set; }
            public string? Color { get; set; }
            public int? DiscountPercentage { get; set; }
            public List<Guid>? CategoryIds { get; set; }
            public List<string>? ProductPictureUrls { get; set; }
        }
        sealed class UpdateProductResponse
        {
            public Guid Id { get; set; }
            public Guid SellerId { get; set; }
            public string ProductCode { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string CoverPictureUrl { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public decimal Weight { get; set; }
            public string Color { get; set; }
            public short DiscountPercentage { get; set; } = 0;
            public List<CategoryResponse> Categories { get; set; }
            public List<ProductPictureResponse> Pictures { get; set; }
        }
        sealed class CategoryResponse
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
        sealed class ProductPictureResponse
        {
            public Guid Id { get; set; }
            public string PictureUrl { get; set; }
        }
        sealed class UpdateProductValidator : Validator<UpdateProductRequest>
        {
            public UpdateProductValidator()
            {
                RuleFor(x => x.Name)
                    .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.")
                    .When(x => x.Name is not null);

                RuleFor(x => x.Description)
                    .MaximumLength(1000).WithMessage("Product description cannot exceed 1000 characters.")
                    .When(x => x.Description is not null);

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

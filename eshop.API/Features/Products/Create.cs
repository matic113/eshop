using eshop.Application.Contracts;
using eshop.Domain.Entities;
using eshop.Infrastructure.Persistence;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace eshop.API.Features.Products
{
    sealed class CreateProductEndpoint : Endpoint<CreateProductRequest, CreateProductResponse>
    {
        private readonly IPublicCodeGenerator _publicCodeGenerator;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateProductEndpoint(IPublicCodeGenerator codeGenerator, IGenericRepository<Product> productRepository, IGenericRepository<Category> categoryRepository, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _publicCodeGenerator = codeGenerator;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public override void Configure()
        {
            Post("/api/products");
            AllowAnonymous();
            Description(x => x
                .WithTags("Products")
                .Accepts<CreateProductRequest>("application/json")
                .Produces<CreateProductResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithName("CreateProduct")
                .WithSummary("Creates a new product."));
        }

        public override async Task HandleAsync(CreateProductRequest r, CancellationToken c)
        {
            var sellerExists = await _userManager.FindByIdAsync(r.SellerId.ToString());

            if (sellerExists is null)
            {
                AddError(x => x.SellerId, "Seller does not exist.");
                await SendErrorsAsync();
                return;
            }

            string productCode = _publicCodeGenerator.Generate("PRD");

            Product product = new Product
            {
                Id = Guid.NewGuid(),
                ProductCode = productCode,
                Name = r.Name,
                Description = r.Description,
                CoverPictureUrl = r.CoverPictureUrl,
                Price = r.Price,
                Stock = r.Stock,
                Weight = r.Weight,
                Color = r.Color,
                DiscountPercentage = (short)r.DiscountPercentage,
                SellerId = r.SellerId
            };

            var existingCategories = await _categoryRepository.GetAllAsync();

            foreach (var categoryId in r.CategoryIds)
            {
                var category = existingCategories.FirstOrDefault(c => c.Id == categoryId);
                if (category != null)
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

            var result = await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(c);

            if (result is not null)
            {
                var response = new CreateProductResponse
                {
                    Id = result.Id,
                    SellerId = result.SellerId,
                    ProductCode = result.ProductCode,
                    Name = result.Name,
                    Description = result.Description,
                    CoverPictureUrl = result.CoverPictureUrl,
                    Price = result.Price,
                    Stock = result.Stock,
                    Weight = result.Weight,
                    Color = result.Color,
                    DiscountPercentage = result.DiscountPercentage,
                    Categories = result.Categories
                                .Select(c => new CategoryResponse { Id = c.Id, Name = c.Name }).ToList(),
                    Pictures = result.ProductPictures
                                .Select(p => new ProductPictureResponse { Id = p.Id, PictureUrl = p.PictureUrl }).ToList()
                };

                await SendAsync(response, StatusCodes.Status201Created);
            }
            else
            {
                AddError("Failed to create product.");
                await SendErrorsAsync();
            }

        }
    }

    sealed class CreateProductRequest
    {
        public Guid SellerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CoverPictureUrl { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal Weight { get; set; }
        public string Color { get; set; }
        public int DiscountPercentage { get; set; } = 0;
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
        public List<string> ProductPictureUrls { get; set; } = new List<string>();
    }

    sealed class CreateProductResponse
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

    sealed class CreateProductValidator : Validator<CreateProductRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Product description is required.")
                .MaximumLength(1000).WithMessage("Product description cannot exceed 1000 characters.");

            RuleFor(x => x.CoverPictureUrl)
                .NotEmpty().WithMessage("Cover picture URL is required.")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("Cover picture URL must be a valid absolute URL.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

            RuleFor(x => x.Weight)
                .GreaterThanOrEqualTo(0).WithMessage("Weight cannot be negative.");

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Color is required.")
                .MaximumLength(20).WithMessage("Color cannot exceed 20 characters.");

            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.");

            RuleFor(x => x.SellerId)
                .NotEmpty().WithMessage("Seller ID is required.");
        }
    }
}

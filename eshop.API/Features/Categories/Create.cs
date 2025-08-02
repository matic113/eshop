using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Categories
{
    sealed class CreateCategoryRequest
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CoverPictureUrl { get; set; } = string.Empty;
    }
    sealed class CreateCategoryResponse
    {
        public string Message { get; set; } = string.Empty;
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CoverPictureUrl { get; set; }
    }
    sealed class CreateCategoryRequestValidator : Validator<CreateCategoryRequest>
    {
        public CreateCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(50).WithMessage("Category name cannot exceed 50 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }

    sealed class Create : Endpoint<CreateCategoryRequest, CreateCategoryResponse>
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Create(IGenericRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }
        public override void Configure()
        {
            Post("/api/categories");
            AllowAnonymous();
            Description(x => x
                .WithTags("Categories")
                .Produces<CreateCategoryResponse>(201)
                .ProducesProblem(400));
        }

        public override async Task HandleAsync(CreateCategoryRequest req, CancellationToken ct)
        {
            var exists = await _categoryRepository.CheckExistsAsync(x => x.Name == req.Name);

            if (exists)
            {
                AddError(x => x.Name, "Category with this name already exists.");
                await SendErrorsAsync(cancellation: ct);
                return;
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = req.Name.ToLower(),
                Description = req.Description,
                CoverPictureUrl = req.CoverPictureUrl
            };

            await _categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync(ct);

            var response = new CreateCategoryResponse
            {
                Message = "Category created successfully.",
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CoverPictureUrl = category.CoverPictureUrl
            };
            await SendOkAsync(response, ct);
        }
    }
}

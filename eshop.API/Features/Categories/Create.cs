using eshop.Application.Contracts;
using eshop.Domain.Entities;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Categories
{
    public record CreateCategoryRequest(string Name);
    public record CreateCategoryResponse(Guid Id, string Name);
    public class CreateCategoryRequestValidator : Validator<CreateCategoryRequest>
    {
        public CreateCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(50).WithMessage("Category name cannot exceed 50 characters.");
        }
    }

    public class Create : Endpoint<CreateCategoryRequest, CreateCategoryResponse>
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        public Create(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
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
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = req.Name
            };

            var createdCategory = await _categoryRepository.AddAsync(category);
            if (createdCategory is not null)
            {
                var response = new CreateCategoryResponse(createdCategory.Id, createdCategory.Name);
                await SendAsync(response, StatusCodes.Status201Created);
            }
            else
            {
                AddError("Failed to create category.");
                await SendErrorsAsync();
            }
        }
    }
}

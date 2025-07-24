using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Categories
{
    public record UpdateCategoryRequest(string newName);
    public record UpdateCategoryResponse(Guid Id, string newName);
    public class UpdateCategoryRequestValidator : Validator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(x => x.newName)
                .NotEmpty().WithMessage("new name is required.")
                .MaximumLength(50).WithMessage("Category name cannot exceed 50 characters.");
        }
    }
    public class Update : Endpoint<UpdateCategoryRequest, UpdateCategoryResponse>
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Update(IGenericRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }
        public override void Configure()
        {
            Put("/api/categories/{Id}");
            AllowAnonymous();
            Description(x => x
                .WithTags("Categories")
                .Produces<UpdateCategoryResponse>(200)
                .ProducesProblem(400)
                .ProducesProblem(404));
        }
        public override async Task HandleAsync(UpdateCategoryRequest req, CancellationToken ct)
        {
            var categoryId = Route<Guid>("Id");
            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category is null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            category.Name = req.newName;
            await _categoryRepository.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync(ct);

            var response = new UpdateCategoryResponse(category.Id, category.Name);
            await SendOkAsync(response, ct);
        }
    }
}

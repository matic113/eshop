using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Categories
{
    public class Update
    {
        sealed class UpdateCategoryRequest
        {
            public string NewName { get; set; } = string.Empty;
            public string NewDescription { get; set; } = string.Empty;
            public string NewCoverPictureUrl { get; set; } = string.Empty;
        }
        sealed class UpdateCategoryResponse
        {
            public string Message { get; set; } = string.Empty;
            public Guid? Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? CoverPictureUrl { get; set; }
        }
        sealed class UpdateCategoryRequestValidator : Validator<UpdateCategoryRequest>
        {
            public UpdateCategoryRequestValidator()
            {
                RuleFor(x => x.NewName)
                    .MaximumLength(50).WithMessage("Category name cannot exceed 50 characters.");
                RuleFor(x => x.NewDescription)
                    .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
                RuleFor(x => x.NewCoverPictureUrl)
                    .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    .WithMessage("Either leave it empty for old cover or provide a valid URL.");
            }
        }
        sealed class UpdateCategoryEndpoint : Endpoint<UpdateCategoryRequest, UpdateCategoryResponse>
        {
            private readonly IGenericRepository<Category> _categoryRepository;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateCategoryEndpoint(IGenericRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
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
                    await SendAsync(new UpdateCategoryResponse
                    {
                        Message = "Category to be updated was not found."
                    },400);
                    return;
                }

                if (!string.IsNullOrEmpty(req.NewName)) category.Name = req.NewName;
                if (!string.IsNullOrEmpty(req.NewDescription)) category.Description = req.NewDescription;
                if (!string.IsNullOrEmpty(req.NewCoverPictureUrl)) category.CoverPictureUrl = req.NewCoverPictureUrl;

                await _categoryRepository.UpdateAsync(category);
                await _unitOfWork.SaveChangesAsync(ct);

                var response = new UpdateCategoryResponse
                {
                    Message = "Category Update Successfully",
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    CoverPictureUrl = category.CoverPictureUrl
                };
                await SendOkAsync(response, ct);
            }
        }
    }
}

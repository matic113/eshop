using eshop.Application.Contracts;
using eshop.Domain.Entities;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Categories
{
    public class Delete : EndpointWithoutRequest
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        public Delete(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public override void Configure()
        {
            Delete("/api/categories/{Id}");
            AllowAnonymous();
            Description(x => x
                .WithTags("Categories")
                .Produces(200)
                .ProducesProblem(400)
                .ProducesProblem(404));
        }
        public override async Task HandleAsync(CancellationToken ct)
        {
            var categoryId = Route<Guid>("Id");

            bool isDeleted = await _categoryRepository.DeleteAsync(categoryId);

            if (isDeleted)
            {
                await SendOkAsync();
            }
            else
            {
                AddError("Category not found or could not be deleted.");
                await SendNotFoundAsync();
            }
        }
    }
}

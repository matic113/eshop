using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using FastEndpoints;

namespace eshop.API.Features.Categories
{
    public record GetCategoryResponse(Guid Id, string Name);
    public class Get : EndpointWithoutRequest<GetCategoryResponse>
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        public Get(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public override void Configure()
        {
            Get("/api/categories/{Id}");
            AllowAnonymous();
            Description(x => x
                .WithTags("Categories")
                .Produces<GetCategoryResponse>(200)
                .ProducesProblem(404));
        }
        public override async Task HandleAsync(CancellationToken ct)
        {
            var categoryId = Route<Guid>("Id");

            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category is not null)
            {
                var response = new GetCategoryResponse(category.Id, category.Name);
                await SendOkAsync(response);
            }
            else
            {
                await SendNotFoundAsync();
            }
        }
    }
}

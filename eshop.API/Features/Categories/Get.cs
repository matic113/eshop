using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using FastEndpoints;

namespace eshop.API.Features.Categories
{
    public class Get
    {
        sealed class GetCategoryResponse
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? CoverPictureUrl { get; set; }
        }
        sealed class GetCategoryEndpoint : EndpointWithoutRequest<GetCategoryResponse>
        {
            private readonly IGenericRepository<Category> _categoryRepository;
            public GetCategoryEndpoint(IGenericRepository<Category> categoryRepository)
            {
                _categoryRepository = categoryRepository;
            }
            public override void Configure()
            {
                Get("/api/categories/{Id}");
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
                    var response = new GetCategoryResponse
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description,
                        CoverPictureUrl = category.CoverPictureUrl
                    };
                    await SendOkAsync(response);
                }
                else
                {
                    await SendNotFoundAsync();
                }
            }
        }
    }
}

using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using FastEndpoints;

namespace eshop.API.Features.Categories
{
    public class ListAll
    {
        sealed class GetAllCategoriesResponse
        {
            public List<CategoryItem> Categories { get; set; } = [];
        }
        sealed class CategoryItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string CoverPictureUrl { get; set; } = string.Empty;
        }

        sealed class GetAllCategoriesEndpoint : EndpointWithoutRequest<GetAllCategoriesResponse>
        {
            private readonly IGenericRepository<Category> _categoryRepository;

            public GetAllCategoriesEndpoint(IGenericRepository<Category> categoryRepository)
            {
                _categoryRepository = categoryRepository;
            }

            public override void Configure()
            {
                Get("/api/categories");
                Description(x => x
                    .WithTags("Categories")
                    .Produces<IEnumerable<GetAllCategoriesResponse>>(200));
            }

            public override async Task HandleAsync(CancellationToken c)
            {
                var categories = await _categoryRepository.GetAllAsync();
                var response = new GetAllCategoriesResponse
                {
                    Categories = categories.Select(c => new CategoryItem
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description ?? "",
                        CoverPictureUrl = c.CoverPictureUrl ?? ""
                    }).ToList()
                };
                await SendOkAsync(response, c);
            }
        }
    }
}

using eshop.Application.Contracts;
using eshop.Domain.Entities;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Categories
{
    //public record ListAllCategories();
    //public class ListAllCategoriesValidator : Validator<ListAllCategories>
    //{
    //    public ListAllCategoriesValidator()
    //    {
    //    }
    //}
    public class ListAll : EndpointWithoutRequest<IEnumerable<GetCategoryResponse>>
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        public ListAll(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public override void Configure()
        {
            Get("/api/categories");
            AllowAnonymous();
            Description(x => x
                .WithTags("Categories")
                .Produces<IEnumerable<GetCategoryResponse>>(200));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var categories = await _categoryRepository.GetAllAsync();
            var response = categories.Select(c => new GetCategoryResponse(c.Id, c.Name)).ToList();

            await SendOkAsync(response);
        }
    }
}

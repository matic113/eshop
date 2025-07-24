using eshop.Application.Contracts;
using eshop.Domain.Entities;
using FastEndpoints;

namespace eshop.API.Features.Categories
{
    public class Delete : EndpointWithoutRequest
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Delete(IGenericRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
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
                await _unitOfWork.SaveChangesAsync(ct);
                await SendOkAsync(ct);
            }
            else
            {
                await SendNotFoundAsync(ct);
            }
        }
    }
}

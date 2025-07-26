using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using FastEndpoints;

namespace eshop.API.Features.Products
{
    sealed class Delete
    {
        sealed class DeleteProductEndpoint : EndpointWithoutRequest
        {
            private readonly IGenericRepository<Product> _productRepository;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteProductEndpoint(IGenericRepository<Product> productRepository, IUnitOfWork unitOfWork)
            {
                _productRepository = productRepository;
                _unitOfWork = unitOfWork;
            }

            public override void Configure()
            {
                Delete("/api/products/{Id}");
                AllowAnonymous();
                Description(x => x
                    .WithTags("Products")
                    .Produces(200)
                    .ProducesProblem(400)
                    .ProducesProblem(404));
            }

            public override async Task HandleAsync(CancellationToken c)
            {
                var productId = Route<Guid>("Id");

                var product = await _productRepository.GetByIdAsync(productId);

                if (product is null)
                {
                    await SendNotFoundAsync();
                    return;
                }

                if (product.IsDeleted)
                {
                    AddError("Product already deleted.");
                    await SendErrorsAsync();
                    return;
                }

                product.IsDeleted = true;
                product.DeletedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync(c);
                await SendOkAsync(new { Message = "Product deleted successfully." }, c);
                return;
            }
        }
    }
}

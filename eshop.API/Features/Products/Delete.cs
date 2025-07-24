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

                bool isDeleted = await _productRepository.DeleteAsync(productId);

                if (isDeleted)
                {
                    await _unitOfWork.SaveChangesAsync(c);
                    await SendOkAsync(c);
                }
                else
                {
                    await SendNotFoundAsync(c);
                }
            }
        }
    }
}

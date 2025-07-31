using ErrorOr;
using eshop.Application.Dtos;

namespace eshop.Application.Contracts.Services
{
    public interface IReviewService
    {
        Task<ErrorOr<ReviewDto>> AddProductReviewAsync(Guid userId,
            Guid productId,
            string? comment,
            int rating);
        Task<ErrorOr<ProductReviewsDto>> GetProductReviewsAsync(Guid productId, int page, int pageSize);
    }
}

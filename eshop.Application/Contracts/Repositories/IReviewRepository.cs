using eshop.Application.Dtos;
using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<Review> AddReviewToProductAsync(Review review);
        Task<Review?> GetReviewByProductIdAndUserIdAsync(Guid productId, Guid userId);
        Task<IEnumerable<short>> GetReviewsRatingByProductIdAsync(Guid productId);
        Task<PagedList<UserReviewDto>> GetReviewsByProductIdAsync(Guid productId, int page, int pageSize);
    }
}

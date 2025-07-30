using eshop.Application.Contracts.Repositories;
using eshop.Application.Dtos;
using eshop.Domain.Entities;
using eshop.Infrastructure.Extensions;
using eshop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eshop.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Review> AddReviewToProductAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);

            return review;
        }

        public async Task<Review?> GetReviewByProductIdAndUserIdAsync(Guid productId, Guid userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId && r.ProductId == productId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<short>> GetReviewsRatingByProductIdAsync(Guid productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Select(r => r.Rating)
                .ToListAsync();
        }
        public async Task<PagedList<UserReviewDto>> GetReviewsByProductIdAsync(Guid productId, int page, int pageSize)
        {
            var reviewsQuery = _context.Reviews
                .Where(r => r.ProductId == productId);

            var reviewsWithUsersQuery = reviewsQuery.Join(
                _context.Users,
                review => review.UserId,
                user => user.Id,
                (review, user) => new UserReviewDto
                {
                    UserName = $"{user.FirstName} {user.LastName}",
                    UserPicture = user.ProfilePicture,
                    Comment = review.Comment,
                    Rating = review.Rating,
                    CreatedAt = review.CreatedAt
                });

            var sortedQuery = reviewsWithUsersQuery
                .OrderByDescending(x => x.CreatedAt);

            return await sortedQuery.ToPagedListAsync(page, pageSize);
        }
    }
}

using System.Net.WebSockets;
using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
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
    }
}

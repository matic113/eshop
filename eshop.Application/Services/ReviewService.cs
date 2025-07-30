using ErrorOr;
using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Application.Contracts.Services;
using eshop.Application.Dtos;
using eshop.Application.Errors;
using eshop.Domain.Entities;
using System.Text;

namespace eshop.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IProductRepository _productRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork, IReviewRepository reviewRepository, IProductRepository productRepository)
        {
            _unitOfWork = unitOfWork;
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
        }

        public async Task<ErrorOr<ReviewDto>> AddProductReviewAsync(Guid userId, Guid productId, string? comment, int rating)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
            {
                return ReviewErrors.ProductNotFound;
            }

            var pastReview = await _reviewRepository
                .GetReviewByProductIdAndUserIdAsync(productId, userId);

            if (pastReview is not null)
            {
                return ReviewErrors.UserAlreadyReviewed;
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                UserId = userId,
                Comment = comment ?? "",
                Rating = (short)rating
            };

            await _reviewRepository.AddReviewToProductAsync(review);

            var reviewsCount = product.ReviewsCount + 1;
            var averageRating = (product.Rating * product.ReviewsCount + rating) / reviewsCount;

            product.Rating = averageRating;
            product.ReviewsCount = reviewsCount;

            await _unitOfWork.SaveChangesAsync();

            return new ReviewDto
            {
                ReviewId = review.Id,
                ProductId = review.ProductId,
                UserId = review.UserId,
                Comment = review.Comment,
                Rating = review.Rating
            };
        }

        public async Task<ErrorOr<PagedList<UserReviewDto>>> GetProductReviewsAsync(Guid productId, int page, int pageSize)
        {
            var productExists = await _productRepository.CheckExistsByIdAsync(productId);

            if (!productExists)
            {
                return ReviewErrors.ProductNotFound;
            }

            return await _reviewRepository.GetReviewsByProductIdAsync(
                productId,
                page,
                pageSize);
        }
    }
}

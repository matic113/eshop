namespace eshop.Application.Dtos
{
    public class ProductReviewsDto
    {
        public float AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public PagedList<UserReviewDto> Reviews { get; set; } = null!;
    }
}

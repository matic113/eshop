using ErrorOr;

namespace eshop.Application.Errors
{
    public static class ReviewErrors
    {
        public static Error UserAlreadyReviewed => Error.Validation(
            code: "Review.UserAlreadyReviewed",
            description: "User has already reviewed this product.");

        public static Error ProductNotFound => Error.NotFound(
            code: "Review.ProductNotFound",
            description: "The specified product was not found.");
    }
}

using ErrorOr;

namespace eshop.Application.Errors
{
    public static class OrderErrors
    {
        public static Error CartIsEmpty => Error.Validation(
            code: "Order.CartIsEmpty",
            description: "Cannot checkout with an empty cart.");

        public static Error AddressNotFound => Error.NotFound(
            code: "Order.AddressNotFound",
            description: "The specified shipping address was not found.");

        public static Error InsufficientStock(string productName, int available) => Error.Validation(
            code: "Order.InsufficientStock",
            description: $"Product '{productName}' does not have enough stock. Only {available} available.");

        public static Error ProductNotFound(Guid productId) => Error.NotFound(
            code: "Order.ProductNotFound",
            description: $"A product with ID '{productId}' in your cart was not found.");

        public static Error PaymentIntentFailed => Error.Failure(
            code: "Order.PaymentIntentFailed",
            description: "Failed to create a payment intent with the payment provider.");
    }
}

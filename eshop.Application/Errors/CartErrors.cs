
using ErrorOr;

namespace eshop.Application.Errors
{
    public static class CartErrors
    {
        public static Error NotFound => Error.NotFound(
            code: "Cart.NotFound",
            description: "Cart not found for user.");

        public static Error Empty => Error.NotFound(
            code: "Cart.Empty",
            description: "Cart is empty.");

        public static Error ProductNotFound => Error.NotFound(
            code: "Cart.ProductNotFound",
            description: "Product not found.");

        public static Error ProductOutOfStock => Error.Validation(
            code: "Cart.ProductOutOfStock",
            description: "Product is out of stock.");

        public static Error NotEnoughStock(int requested, int available) => Error.Validation(
            code: "Cart.NotEnoughStock",
            description: $"Not enough stock for product, requested: {requested}, available: {available}.");

        public static Error ItemNotFound => Error.NotFound(
            code: "Cart.ItemNotFound",
            description: "Item was not found in cart.");
    }
}

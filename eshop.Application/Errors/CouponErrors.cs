
using ErrorOr;

namespace eshop.Application.Errors
{
    public static class CouponErrors
    {
        public static Error NotFound => Error.NotFound(
            code: "Coupon.NotFound",
            description: "Coupon not found.");
        public static Error InvalidCouponCode => Error.Validation(
            code: "Coupon.InvalidCode",
            description: "Invalid coupon code.");
        public static Error CouponExpired => Error.Validation(
            code: "Coupon.Expired",
            description: "Coupon has expired.");
        public static Error CouponUsageLimitExceeded => Error.Validation(
            code: "Coupon.UsageLimitExceeded",
            description: "Coupon usage limit exceeded.");
        public static Error UserLimitExcedeed => Error.Validation(
            code: "Coupon.UserLimitExceeded",
            description: "Coupon has already been used the maximum allowed times by this user.");
        public static Error CouponAlreadyExists => Error.Validation(
            code: "Coupon.AlreadyExists",
            description: "A coupon with same code already exists.");
        public static Error InvalidCouponType => Error.Validation(
            code: "Coupon.InvalidCouponType",
            description: "Coupon Type is invalid.");
    }
}

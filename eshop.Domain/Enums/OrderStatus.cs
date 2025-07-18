namespace eshop.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,      // Order created, waiting for payment
        Completed,    // Payment successful
        Failed,       // Payment failed
        Processing,   // Seller is preparing the order
        Shipped,      // Order has been shipped
        Delivered,    // Order has been delivered to the customer
        Cancelled     // Order has been cancelled
    }
}

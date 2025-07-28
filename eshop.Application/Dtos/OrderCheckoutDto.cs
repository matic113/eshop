using eshop.Domain.Entities;

namespace eshop.Application.Dtos
{
    public class OrderCheckoutDto
    {
        public Order Order { get; set; } = null!;
        public string UnifiedCheckoutUrl { get; set; } = "";
        public string PaymentClientSecret { get; set; } = "";
    }
}

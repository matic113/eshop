using eshop.Domain.Enums;

namespace eshop.Domain.Entities
{
    public class OrderStatusHistory : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public required string OrderCode { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime ChangeDate { get; set; }
        public string Notes { get; set; } = string.Empty;

        // Navigational properties
        public Order Order { get; set; } = null!;
    }
}

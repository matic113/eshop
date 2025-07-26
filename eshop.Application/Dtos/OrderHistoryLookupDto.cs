using eshop.Domain.Entities;

namespace eshop.Application.Dtos
{
    public class OrderHistoryLookupDto
    {
        public string? OrderCode { get; set; }
        public IEnumerable<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    }
}

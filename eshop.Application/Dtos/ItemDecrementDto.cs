using eshop.Domain.Entities;

namespace eshop.Application.Dtos
{
    public class ItemDecrementDto
    {
        public bool WasRemoved { get; set; } = false;
        public CartItem? CartItem { get; set; }
    }
}

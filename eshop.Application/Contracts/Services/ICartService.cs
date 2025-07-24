using ErrorOr;
using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Services
{
    public interface ICartService
    {
        Task<ErrorOr<CartItem>> AddItemToUserCartAsync(Guid userId, Guid productId, int quantity = 1);
    }
}

using ErrorOr;
using eshop.Application.Dtos;
using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Services
{
    public interface ICartService
    {
        Task<ErrorOr<CartItem>> AddItemToUserCartAsync(Guid userId, Guid productId, int quantity = 1);
        Task<ErrorOr<ItemDecrementDto>> DecrementItemQuantityInUserCartAsync(Guid userId, Guid itemId, int quantity = 1);
    }
}

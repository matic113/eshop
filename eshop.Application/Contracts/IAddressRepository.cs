using eshop.Domain.Entities;

namespace eshop.Application.Contracts
{
    public interface IAddressRepository : IGenericRepository<Address>
    {
        Task<IEnumerable<Address>> GetUserAddressesAsync(Guid userId);
    }
}

using eshop.Domain.Entities;

namespace eshop.Application.Contracts
{
    public interface IGenericRepository<T> where T : IBaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
    }
}

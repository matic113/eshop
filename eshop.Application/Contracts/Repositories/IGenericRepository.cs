using eshop.Domain.Entities;
using System.Linq.Expressions;

namespace eshop.Application.Contracts.Repositories
{
    public interface IGenericRepository<T> where T : IBaseEntity
    {
        Task<bool> CheckExistsAsync(Expression<Func<T, bool>> predicate);
        Task<bool> CheckExistsByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
    }
}

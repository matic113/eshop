using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Repositories
{
    public interface IVerificationTokensRepository : IGenericRepository<VerificationToken>
    {
        Task<VerificationToken?> GetByUserIdAndCodeAsync(Guid userId, string code);
        Task DeleteUserTokensAsync(Guid userId);
    }
}

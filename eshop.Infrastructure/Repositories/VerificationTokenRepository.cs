using eshop.Application.Contracts;
using eshop.Domain.Entities;
using eshop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eshop.Infrastructure.Repositories
{
    public class VerificationTokenRepository : GenericRepository<VerificationToken>, IVerificationTokensRepository
    {
        public VerificationTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<VerificationToken?> GetByUserIdAndCodeAsync(Guid userId, string code)
        {
            return await _context.VerificationTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Code == code);
        }

        public async Task DeleteUserTokensAsync(Guid userId)
        {
            var tokens = await _context.VerificationTokens
                .Where(t => t.UserId == userId)
                .ToListAsync();

            if (tokens.Any())
            {
                _context.VerificationTokens.RemoveRange(tokens);
            }
        }
    }
}

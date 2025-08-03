using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using eshop.Infrastructure.Extensions;
using eshop.Infrastructure.Persistence;

namespace eshop.Infrastructure.Repositories
{
    public class OfferRepository : GenericRepository<Offer>, IOfferRepository
    {
        public OfferRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<PagedList<Offer>> GetAllOffers(int pageNumber = 1, int pageSize = 10)
        {
            var offers = await _context.Offers
                .OrderBy(o => o.CreatedAt)
                .ToPagedListAsync(pageNumber, pageSize);

            return offers;
        }
    }
}

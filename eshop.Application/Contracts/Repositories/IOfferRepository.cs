using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Repositories
{
    public interface IOfferRepository: IGenericRepository<Offer>
    {
        Task<PagedList<Offer>> GetAllOffers(
            int pageNumber = 1, 
            int pageSize = 10);
    }
}

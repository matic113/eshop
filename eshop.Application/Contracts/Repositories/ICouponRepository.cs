using eshop.Application.Dtos;
using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Repositories
{
    public interface ICouponRepository : IGenericRepository<Coupon>
    {
        Task<List<CouponDto>> GetAllCouponsAsync();
    }
}

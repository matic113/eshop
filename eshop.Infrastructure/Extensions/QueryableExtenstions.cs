using Microsoft.EntityFrameworkCore;

namespace eshop.Infrastructure.Extensions
{
    public static class QueryableExtenstions
    {
        public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize)
        {
            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<T>(items, page, pageSize, totalCount);
        }

    }
}

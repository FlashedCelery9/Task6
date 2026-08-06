using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Task6.Helpers.Pagination;

public static class PaginationHelper
{
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        int page,
        int size,
        IConfigurationProvider mapperConfig
    )
    {
        int totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ProjectTo<TDto>(mapperConfig)
            .ToListAsync();

        return new PagedResult<TDto>(items, totalCount, page, size);
    }
}
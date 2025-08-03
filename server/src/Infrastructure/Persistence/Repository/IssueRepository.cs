using Application.Contracts.Persistence;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class IssueRepository : GenericRepository<Issue>, IIssueRepository
{
    public IssueRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<PagedResult<Issue>> GetAllIssuesPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        // Apply search filter
        query = ApplySearchFilter(query, parameters.Search);

        // Sorting
        if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            var sortBy = parameters.SortBy.ToLower();
            var descending = parameters.SortDescending;
            query = sortBy switch
            {
                "title" => descending ? query.OrderByDescending(i => i.Title) : query.OrderBy(i => i.Title),
                "status" => descending ? query.OrderByDescending(i => i.Status) : query.OrderBy(i => i.Status),
                "prioritylevel" => descending ? query.OrderByDescending(i => i.PriorityLevel) : query.OrderBy(i => i.PriorityLevel),
                "category" => descending ? query.OrderByDescending(i => i.Category) : query.OrderBy(i => i.Category),
                "reporteddate" => descending ? query.OrderByDescending(i => i.ReportedDate) : query.OrderBy(i => i.ReportedDate),
                "resolveddate" => descending ? query.OrderByDescending(i => i.ResolvedDate) : query.OrderBy(i => i.ResolvedDate),
                "createdat" => descending ? query.OrderByDescending(i => i.CreatedAt) : query.OrderBy(i => i.CreatedAt),
                "updatedat" => descending ? query.OrderByDescending(i => i.UpdatedAt) : query.OrderBy(i => i.UpdatedAt),
                _ => query
            };
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Include(i => i.ReportedByUser)
            .Include(i => i.ResolvedByUser)
            .Include(i => i.Vehicle)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<Issue>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<Issue> ApplySearchFilter(IQueryable<Issue> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return query;

        string searchPattern = $"%{searchText.Trim().ToLowerInvariant()}%";

        return query.Where(i =>
            EF.Functions.Like(i.Title, searchPattern) ||
            EF.Functions.Like(i.Description ?? string.Empty, searchPattern) ||
            EF.Functions.Like(i.IssueNumber.ToString(), searchPattern)
        );
    }

    public async Task<Issue?> GetIssueWithDetailsAsync(int issueID)
    {
        return await _dbSet
            .Include(i => i.ReportedByUser)
            .Include(i => i.ResolvedByUser)
            .Include(i => i.Vehicle)
            .FirstOrDefaultAsync(i => i.ID == issueID);
    }
}
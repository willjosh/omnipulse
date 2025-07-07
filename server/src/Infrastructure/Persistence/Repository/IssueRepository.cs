using System.Linq;
using System.Threading.Tasks;

using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class IssueRepository : GenericRepository<Issue>, IIssueRepository
{
    private readonly OmnipulseDatabaseContext _context;
    public IssueRepository(OmnipulseDatabaseContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PagedResult<Issue>> GetAllIssuesPagedAsync(PaginationParameters parameters)
    {
        var query = _context.Issues.AsQueryable();

        // Filtering (search by title or description)
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLower();
            query = query.Where(i =>
                (i.Title != null && i.Title.ToLower().Contains(search)) ||
                (i.Description != null && i.Description.ToLower().Contains(search)) ||
                i.Category.ToString().ToLower().Contains(search) ||
                i.PriorityLevel.ToString().ToLower().Contains(search) ||
                i.Status.ToString().ToLower().Contains(search) ||
                i.VehicleID.ToString().Contains(search) ||
                i.IssueNumber.ToString().Contains(search) ||
                (i.ReportedByUserID != null && i.ReportedByUserID.ToLower().Contains(search))
            );
        }

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
}
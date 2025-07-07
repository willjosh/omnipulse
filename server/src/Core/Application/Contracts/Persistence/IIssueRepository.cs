using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IIssueRepository : IGenericRepository<Issue>
{
    Task<PagedResult<Issue>> GetAllIssuesPagedAsync(PaginationParameters parameters);
}
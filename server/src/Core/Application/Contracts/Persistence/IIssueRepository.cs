using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IIssueRepository : IGenericRepository<Issue>
{
    public Task<PagedResult<Issue>> GetAllIssuesPagedAsync(PaginationParameters parameters);
    public Task<Issue?> GetIssueWithDetailsAsync(int issueID);
}
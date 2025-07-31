using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IInspectionFormRepository : IGenericRepository<InspectionForm>
{
    Task<bool> IsTitleUniqueAsync(string title);
    Task<PagedResult<InspectionForm>> GetAllInspectionFormsPagedAsync(PaginationParameters parameters);
    Task<InspectionForm?> GetInspectionFormWithItemsAsync(int inspectionFormId);
}
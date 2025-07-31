using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

namespace Application.Contracts.Persistence;

public interface IInspectionFormItemRepository : IGenericRepository<InspectionFormItem>
{
    Task<PagedResult<InspectionFormItem>> GetAllInspectionFormItemsPagedAsync(PaginationParameters parameters);

    // Query methods
    Task<IReadOnlyList<InspectionFormItem>> GetItemsByFormIdAsync(int inspectionFormId);
    Task<IReadOnlyList<InspectionFormItem>> GetRequiredItemsByFormIdAsync(int inspectionFormId);
    Task<IReadOnlyList<InspectionFormItem>> GetItemsByTypeAsync(InspectionFormItemTypeEnum itemType);

    // Statistics methods
    Task<int> CountItemsByFormIdAsync(int inspectionFormId);
    Task<int> CountRequiredItemsByFormIdAsync(int inspectionFormId);
}
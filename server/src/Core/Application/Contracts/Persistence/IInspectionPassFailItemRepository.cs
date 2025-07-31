using Domain.Entities;

namespace Application.Contracts.Persistence;

/// <summary>
/// Repository interface for <see cref="InspectionPassFailItem"/> entity.
/// Note: This entity uses a composite primary key (InspectionID, InspectionFormItemID)
/// and does not inherit from BaseEntity, so it doesn't use <see cref="IGenericRepository{T}"/>.
/// </summary>
public interface IInspectionPassFailItemRepository
{
    // Basic CRUD operations

    /// <summary>Gets a specific <see cref="InspectionPassFailItem"/> by its composite key.</summary>
    Task<InspectionPassFailItem?> GetByCompositeKeyAsync(int inspectionId, int inspectionFormItemId);

    /// <summary>Retrieves all <see cref="InspectionPassFailItem"/> records.</summary>
    Task<IReadOnlyList<InspectionPassFailItem>> GetAllAsync();

    /// <summary>Adds a new <see cref="InspectionPassFailItem"/> to the context.</summary>
    Task<InspectionPassFailItem> AddAsync(InspectionPassFailItem entity);

    /// <summary>Adds multiple <see cref="InspectionPassFailItem"/> records to the context.</summary>
    Task<IEnumerable<InspectionPassFailItem>> AddRangeAsync(IEnumerable<InspectionPassFailItem> entities);

    /// <summary>Updates an existing <see cref="InspectionPassFailItem"/>.</summary>
    void Update(InspectionPassFailItem entity);

    /// <summary>Deletes a specified <see cref="InspectionPassFailItem"/>.</summary>
    void Delete(InspectionPassFailItem entity);

    /// <summary>Deletes a <see cref="InspectionPassFailItem"/> using its composite key.</summary>
    void DeleteByCompositeKey(int inspectionId, int inspectionFormItemId);

    /// <summary>Persists all changes made in the context to the database.</summary>
    Task<int> SaveChangesAsync();

    // Query operations

    /// <summary>Gets all <see cref="InspectionPassFailItem"/> entries for a specific <see cref="Inspection"/>.</summary>
    Task<IReadOnlyList<InspectionPassFailItem>> GetByInspectionIdAsync(int inspectionId);

    /// <summary>Gets all <see cref="InspectionPassFailItem"/> entries by <see cref="InspectionFormItem"/> ID.</summary>
    Task<IReadOnlyList<InspectionPassFailItem>> GetByInspectionFormItemIdAsync(int inspectionFormItemId);

    /// <summary>Gets all passed <see cref="InspectionPassFailItem"/> entries for a specific <see cref="Inspection"/>.</summary>
    Task<IReadOnlyList<InspectionPassFailItem>> GetPassedItemsByInspectionIdAsync(int inspectionId);

    /// <summary>Gets all failed <see cref="InspectionPassFailItem"/> entries for a specific <see cref="Inspection"/>.</summary>
    Task<IReadOnlyList<InspectionPassFailItem>> GetFailedItemsByInspectionIdAsync(int inspectionId);

    // Existence checks

    /// <summary>Checks whether a specific <see cref="InspectionPassFailItem"/> exists.</summary>
    Task<bool> ExistsAsync(int inspectionId, int inspectionFormItemId);

    /// <summary>Checks whether all specified <see cref="InspectionPassFailItem"/> exist for the given <see cref="Inspection"/>.</summary>
    Task<bool> AllExistForInspectionAsync(int inspectionId, IEnumerable<int> inspectionFormItemIds);

    // Statistics

    /// <summary>Counts all <see cref="InspectionPassFailItem"/> entries for a given <see cref="Inspection"/>.</summary>
    Task<int> CountByInspectionIdAsync(int inspectionId);

    /// <summary>Counts all passed <see cref="InspectionPassFailItem"/> entries for a given <see cref="Inspection"/>.</summary>
    Task<int> CountPassedByInspectionIdAsync(int inspectionId);

    /// <summary>Counts all failed <see cref="InspectionPassFailItem"/> entries for a given <see cref="Inspection"/>.</summary>
    Task<int> CountFailedByInspectionIdAsync(int inspectionId);

    /// <summary>Calculates the pass rate for a given <see cref="Inspection"/> based on <see cref="InspectionPassFailItem"/> entries.</summary>
    Task<double> GetPassRateByInspectionIdAsync(int inspectionId);
}
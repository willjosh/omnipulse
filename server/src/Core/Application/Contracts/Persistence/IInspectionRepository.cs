using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IInspectionRepository : IGenericRepository<Inspection>
{
    // Paged Results
    Task<PagedResult<Inspection>> GetAllInspectionsPagedAsync(PaginationParameters parameters);

    // Get inspection with related data
    Task<Inspection?> GetInspectionWithDetailsAsync(int inspectionId);

    // Query methods
    Task<IReadOnlyList<Inspection>> GetInspectionsByVehicleIdAsync(int vehicleId);
    Task<IReadOnlyList<Inspection>> GetInspectionsByTechnicianIdAsync(string technicianId);
    Task<IReadOnlyList<Inspection>> GetInspectionsByFormIdAsync(int inspectionFormId);
    Task<IReadOnlyList<Inspection>> GetInspectionsByDateRangeAsync(DateTime startDate, DateTime endDate);

    // Statistics methods
    Task<int> CountInspectionsByVehicleIdAsync(int vehicleId);
    Task<int> CountInspectionsByTechnicianIdAsync(string technicianId);
}
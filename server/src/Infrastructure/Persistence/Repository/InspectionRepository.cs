using Application.Contracts.Persistence;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class InspectionRepository : GenericRepository<Inspection>, IInspectionRepository
{
    public InspectionRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<PagedResult<Inspection>> GetAllInspectionsPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        // Apply search filter
        query = ApplySearchFilter(query, parameters.Search);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        // Apply includes for related entities
        query = query
            .Include(i => i.Vehicle)
            .Include(i => i.User)
            .Include(i => i.InspectionForm)
            .Include(i => i.InspectionPassFailItems);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<Inspection>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<Inspection> ApplySearchFilter(IQueryable<Inspection> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return query;

        string searchPattern = $"%{searchText.Trim().ToLowerInvariant()}%";

        return query.Where(i =>
            EF.Functions.Like(i.Notes ?? string.Empty, searchPattern) ||
            EF.Functions.Like(i.TechnicianID, searchPattern) ||
            EF.Functions.Like(i.VehicleID.ToString(), searchPattern) ||
            EF.Functions.Like(i.Vehicle.Name, searchPattern) ||
            EF.Functions.Like(i.User.FirstName, searchPattern) ||
            EF.Functions.Like(i.User.LastName, searchPattern) ||
            EF.Functions.Like(i.InspectionForm.Title, searchPattern)
        );
    }

    private static IQueryable<Inspection> ApplySorting(IQueryable<Inspection> query, string? sortBy, bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderByDescending(i => i.InspectionStartTime); // Default sort
        }

        var sortByLower = sortBy.ToLower();
        return sortByLower switch
        {
            "inspectionstarttime" => sortDescending
                ? query.OrderByDescending(i => i.InspectionStartTime)
                : query.OrderBy(i => i.InspectionStartTime),
            "inspectionendtime" => sortDescending
                ? query.OrderByDescending(i => i.InspectionEndTime)
                : query.OrderBy(i => i.InspectionEndTime),
            "vehiclecondition" => sortDescending
                ? query.OrderByDescending(i => i.VehicleCondition)
                : query.OrderBy(i => i.VehicleCondition),
            "odometerreading" => sortDescending
                ? query.OrderByDescending(i => i.OdometerReading)
                : query.OrderBy(i => i.OdometerReading),
            "vehicleid" => sortDescending
                ? query.OrderByDescending(i => i.VehicleID)
                : query.OrderBy(i => i.VehicleID),
            "technicianid" => sortDescending
                ? query.OrderByDescending(i => i.TechnicianID)
                : query.OrderBy(i => i.TechnicianID),
            "createdat" => sortDescending
                ? query.OrderByDescending(i => i.CreatedAt)
                : query.OrderBy(i => i.CreatedAt),
            "updatedat" => sortDescending
                ? query.OrderByDescending(i => i.UpdatedAt)
                : query.OrderBy(i => i.UpdatedAt),
            _ => query.OrderByDescending(i => i.InspectionStartTime) // Default sort for unrecognized fields
        };
    }

    public async Task<Inspection?> GetInspectionWithDetailsAsync(int inspectionId)
    {
        return await _dbSet
            .Include(i => i.Vehicle)
            .Include(i => i.User)
            .Include(i => i.InspectionForm)
            .Include(i => i.InspectionPassFailItems)
                .ThenInclude(pf => pf.InspectionFormItem)
            .FirstOrDefaultAsync(i => i.ID == inspectionId);
    }

    public async Task<IReadOnlyList<Inspection>> GetInspectionsByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet
            .Where(i => i.VehicleID == vehicleId)
            .OrderByDescending(i => i.InspectionStartTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Inspection>> GetInspectionsByTechnicianIdAsync(string technicianId)
    {
        return await _dbSet
            .Where(i => i.TechnicianID == technicianId)
            .OrderByDescending(i => i.InspectionStartTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Inspection>> GetInspectionsByFormIdAsync(int inspectionFormId)
    {
        return await _dbSet
            .Where(i => i.InspectionFormID == inspectionFormId)
            .OrderByDescending(i => i.InspectionStartTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Inspection>> GetInspectionsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(i => i.InspectionStartTime >= startDate && i.InspectionStartTime <= endDate)
            .OrderByDescending(i => i.InspectionStartTime)
            .ToListAsync();
    }

    public async Task<int> CountInspectionsByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet.CountAsync(i => i.VehicleID == vehicleId);
    }

    public async Task<int> CountInspectionsByTechnicianIdAsync(string technicianId)
    {
        return await _dbSet.CountAsync(i => i.TechnicianID == technicianId);
    }
}
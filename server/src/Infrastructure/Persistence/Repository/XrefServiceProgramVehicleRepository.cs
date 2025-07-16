using Application.Contracts.Persistence;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class XrefServiceProgramVehicleRepository : IXrefServiceProgramVehicleRepository
{
    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<XrefServiceProgramVehicle> _dbSet;

    public XrefServiceProgramVehicleRepository(OmnipulseDatabaseContext context)
    {
        _dbContext = context;
        _dbSet = _dbContext.Set<XrefServiceProgramVehicle>();
    }

    public async Task<List<XrefServiceProgramVehicle>> GetByServiceProgramIDAsync(int serviceProgramId)
    {
        return await _dbSet.Where(x => x.ServiceProgramID == serviceProgramId).ToListAsync();
    }

    public async Task<List<XrefServiceProgramVehicle>> GetByVehicleIDAsync(int vehicleId)
    {
        return await _dbSet.Where(x => x.VehicleID == vehicleId).ToListAsync();
    }

    public async Task<bool> ExistsAsync(int serviceProgramId, int vehicleId)
    {
        return await _dbSet.AnyAsync(x => x.ServiceProgramID == serviceProgramId && x.VehicleID == vehicleId);
    }

    public async Task AddAsync(XrefServiceProgramVehicle xref)
    {
        await _dbSet.AddAsync(xref);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<XrefServiceProgramVehicle> xrefs)
    {
        await _dbSet.AddRangeAsync(xrefs);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(int serviceProgramId, int vehicleId)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(x => x.ServiceProgramID == serviceProgramId && x.VehicleID == vehicleId);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RemoveAllForServiceProgramAsync(int serviceProgramId)
    {
        var xrefs = await _dbSet.Where(x => x.ServiceProgramID == serviceProgramId).ToListAsync();
        if (xrefs.Count != 0)
        {
            _dbSet.RemoveRange(xrefs);
            await _dbContext.SaveChangesAsync();
        }
    }
}
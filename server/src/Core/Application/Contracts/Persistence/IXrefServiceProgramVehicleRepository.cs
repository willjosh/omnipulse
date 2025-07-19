using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IXrefServiceProgramVehicleRepository
{
    /// <summary>
    /// Gets all xref records for a given <see cref="ServiceProgram"/>.
    /// </summary>
    /// <param name="serviceProgramID">The ID of the Service Program.</param>
    /// <returns>A list of xref records associated with the specified Service Program.</returns>
    Task<List<XrefServiceProgramVehicle>> GetByServiceProgramIDAsync(int serviceProgramID);

    /// <summary>
    /// Gets paged xref records for a given <see cref="ServiceProgram"/>.
    /// </summary>
    /// <param name="serviceProgramID">The ID of the Service Program.</param>
    /// <param name="parameters">Pagination parameters.</param>
    /// <returns>A paged result of ServiceProgram-Vehicle xref records associated with the specified Service Program.</returns>
    Task<PagedResult<XrefServiceProgramVehicle>> GetAllByServiceProgramIDPagedAsync(int serviceProgramID, PaginationParameters parameters);

    /// <summary>
    /// Gets all xref records for a given <see cref="Vehicle"/>.
    /// </summary>
    /// <param name="vehicleID">The ID of the vehicle.</param>
    /// <returns>A list of xref records associated with the specified vehicle.</returns>
    Task<List<XrefServiceProgramVehicle>> GetByVehicleIDAsync(int vehicleID);

    /// <summary>
    /// Checks if a link exists between a Service Program and a vehicle.
    /// </summary>
    /// <param name="serviceProgramID">The ID of the Service Program.</param>
    /// <param name="vehicleID">The ID of the vehicle.</param>
    /// <returns>True if the link exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int serviceProgramID, int vehicleID);

    /// <summary>
    /// Adds a new link between a Service Program and a vehicle.
    /// </summary>
    /// <param name="xref">The xref entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(XrefServiceProgramVehicle xref);

    /// <summary>
    /// Adds a range of links between a Service Program and vehicles.
    /// </summary>
    /// <param name="xrefs">The xref entities to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddRangeAsync(IEnumerable<XrefServiceProgramVehicle> xrefs);

    /// <summary>
    /// Removes a link between a Service Program and a vehicle.
    /// </summary>
    /// <param name="serviceProgramID">The ID of the Service Program.</param>
    /// <param name="vehicleID">The ID of the vehicle.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(int serviceProgramID, int vehicleID);

    /// <summary>
    /// Removes all links for a given Service Program.
    /// </summary>
    /// <param name="serviceProgramId">The ID of the Service Program.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAllForServiceProgramAsync(int serviceProgramId);
}
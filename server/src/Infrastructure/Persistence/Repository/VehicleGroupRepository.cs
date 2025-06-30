using System;
using Application.Contracts.Persistence;
using Domain.Entities;
using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class VehicleGroupRepository: GenericRepository<VehicleGroup>, IVehicleGroupRepository
{
    public VehicleGroupRepository(OmnipulseDatabaseContext context) : base(context) { }
}

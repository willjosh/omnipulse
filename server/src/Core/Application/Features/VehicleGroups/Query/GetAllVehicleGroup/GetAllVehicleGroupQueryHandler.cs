using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;
using AutoMapper;
using MediatR;

namespace Application.Features.VehicleGroups.Query.GetAllVehicleGroup;

public class GetAllVehicleGroupQueryHandler : IRequestHandler<GetAllVehicleGroupQuery, PagedResult<GetAllVehicleGroupDTO>>
{
    private readonly IVehicleGroupRepository _vehicleGroupRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllVehicleGroupQueryHandler> _logger;

    public GetAllVehicleGroupQueryHandler(IVehicleGroupRepository vehicleGroupRepository, IMapper mapper, IAppLogger<GetAllVehicleGroupQueryHandler> logger)
    {
        _vehicleGroupRepository = vehicleGroupRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<PagedResult<GetAllVehicleGroupDTO>> Handle(GetAllVehicleGroupQuery request, CancellationToken cancellationToken)
    {
        // Validate the request
        throw new NotImplementedException();
    }
}

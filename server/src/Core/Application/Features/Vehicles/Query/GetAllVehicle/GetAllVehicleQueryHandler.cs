using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Models.PaginationModels;
using AutoMapper;
using MediatR;

namespace Application.Features.Vehicles.Query.GetAllVehicle;

public class GetAllVehicleQueryHandler : IRequestHandler<GetAllVehicleQuery, PagedResult<GetAllVehicleDTO>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllVehicleQueryHandler> _logger;

    public GetAllVehicleQueryHandler(IVehicleRepository vehicleRepository, IMapper mapper, IAppLogger<GetAllVehicleQueryHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<PagedResult<GetAllVehicleDTO>> Handle(GetAllVehicleQuery request, CancellationToken cancellationToken)
    {
        // Validate the request
        throw new NotImplementedException();
    }
}

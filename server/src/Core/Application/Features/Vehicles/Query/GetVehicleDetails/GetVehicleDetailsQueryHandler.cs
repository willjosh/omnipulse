using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using AutoMapper;
using MediatR;

namespace Application.Features.Vehicles.Query.GetVehicleDetails;

public class GetVehicleDetailsQueryHandler : IRequestHandler<GetVehicleDetailsQuery, GetVehicleDetailsDTO>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetVehicleDetailsQueryHandler> _logger;

    public GetVehicleDetailsQueryHandler(IVehicleRepository vehicleRepository, IMapper mapper, IAppLogger<GetVehicleDetailsQueryHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<GetVehicleDetailsDTO> Handle(GetVehicleDetailsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

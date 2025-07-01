using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

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

    public async Task<GetVehicleDetailsDTO> Handle(GetVehicleDetailsQuery request, CancellationToken cancellationToken)
    {
        // Get vehicle by id
        _logger.LogInformation($"GetVehicleDetailsQuery for VehicleID: {request.VehicleID}");
        var vehicle = await _vehicleRepository.GetVehicleWithDetailsAsync(request.VehicleID);

        // check if vehicle exists
        if (vehicle == null)
        {
            _logger.LogError($"Vehicle with ID {request.VehicleID} not found.");
            throw new EntityNotFoundException(typeof(Vehicle).ToString(), "VehicleID", request.VehicleID.ToString());
        }

        // map to GetVehicleDetailsDTO
        var vehicleDetailsDto = _mapper.Map<GetVehicleDetailsDTO>(vehicle);


        // return GetVehicleDetailsDTO
        _logger.LogInformation($"Returning VehicleDetailsDTO for VehicleID: {request.VehicleID}");
        return vehicleDetailsDto;
    }
}
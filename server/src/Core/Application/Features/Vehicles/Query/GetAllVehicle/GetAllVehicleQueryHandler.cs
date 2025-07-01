using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.Vehicles.Query.GetAllVehicle;

public class GetAllVehicleQueryHandler : IRequestHandler<GetAllVehicleQuery, PagedResult<GetAllVehicleDTO>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllVehicleQueryHandler> _logger;
    private readonly IValidator<GetAllVehicleQuery> _validator;

    public GetAllVehicleQueryHandler(IVehicleRepository vehicleRepository, IMapper mapper, IAppLogger<GetAllVehicleQueryHandler> logger, IValidator<GetAllVehicleQuery> validator)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<PagedResult<GetAllVehicleDTO>> Handle(GetAllVehicleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllVehicleQuery");
        // validate the request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"GetAllVehicle - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // get all vehicles from the repository
        var result = await _vehicleRepository.GetAllVehiclesPagedAsync(request.Parameters);

        // map the vehicles to DTOs
        var vehicleDTOs = _mapper.Map<List<GetAllVehicleDTO>>(result.Items);

        var pagedResult = new PagedResult<GetAllVehicleDTO>
        {
            Items = vehicleDTOs,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} vehicles for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }
}
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models;
using Application.Models.PaginationModels;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Application.Features.VehicleGroups.Query.GetAllVehicleGroup;

public class GetAllVehicleGroupQueryHandler : IRequestHandler<GetAllVehicleGroupQuery, PagedResult<GetAllVehicleGroupDTO>>
{
    private readonly IVehicleGroupRepository _vehicleGroupRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllVehicleGroupQueryHandler> _logger;
    private readonly IValidator<GetAllVehicleGroupQuery> _validator;
    public GetAllVehicleGroupQueryHandler(IVehicleGroupRepository vehicleGroupRepository, IMapper mapper, IAppLogger<GetAllVehicleGroupQueryHandler> logger, IValidator<GetAllVehicleGroupQuery> validator)
    {
        _vehicleGroupRepository = vehicleGroupRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<PagedResult<GetAllVehicleGroupDTO>> Handle(GetAllVehicleGroupQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllVehicleGroupQuery");
        // validate the request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"GetAllVehicleGroup - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // get all vehicle groups from the repository
        var result = await _vehicleGroupRepository.GetAllVehicleGroupsPagedAsync(request.Parameters);

        // map the vehicle groups to DTOs
        var vehicleGroupDTOs = _mapper.Map<List<GetAllVehicleGroupDTO>>(result.Items);

        var pagedResult = new PagedResult<GetAllVehicleGroupDTO>
        {
            Items = vehicleGroupDTOs,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} vehicle groups for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }
}

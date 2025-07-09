using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.MaintenanceHistories.Query.GetAllMaintenanceHistories;

public class GetAllMaintenanceHistoriesQueryHandler : IRequestHandler<GetAllMaintenanceHistoriesQuery, PagedResult<GetAllMaintenanceHistoryDTO>>
{
    private readonly IMaintenanceHistoryRepository _maintenanceHistoryRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllMaintenanceHistoriesQueryHandler> _logger;
    private readonly IValidator<GetAllMaintenanceHistoriesQuery> _validator;

    public GetAllMaintenanceHistoriesQueryHandler(
        IMaintenanceHistoryRepository maintenanceHistoryRepository,
        IMapper mapper,
        IAppLogger<GetAllMaintenanceHistoriesQueryHandler> logger,
        IValidator<GetAllMaintenanceHistoriesQuery> validator)
    {
        _maintenanceHistoryRepository = maintenanceHistoryRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<PagedResult<GetAllMaintenanceHistoryDTO>> Handle(GetAllMaintenanceHistoriesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllMaintenanceHistoriesQuery");
        // validate the request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"GetAllMaintenanceHistories - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // get all maintenance histories from the repository
        var result = await _maintenanceHistoryRepository.GetAllMaintenanceHistoriesPagedAsync(request.Parameters);

        // map the maintenance histories to DTOs
        var maintenanceHistoryDTOs = _mapper.Map<List<GetAllMaintenanceHistoryDTO>>(result.Items);

        var pagedResult = new PagedResult<GetAllMaintenanceHistoryDTO>
        {
            Items = maintenanceHistoryDTOs,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} maintenance histories for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceTasks.Query.GetAllServiceTask;
using Application.Models;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.ServiceSchedules.Query.GetAllServiceSchedule;

public class GetAllServiceScheduleQueryHandler : IRequestHandler<GetAllServiceScheduleQuery, PagedResult<GetAllServiceScheduleDTO>>
{
    private readonly IServiceScheduleRepository _serviceScheduleRepository;
    private readonly IValidator<GetAllServiceScheduleQuery> _validator;
    private readonly IAppLogger<GetAllServiceScheduleQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetAllServiceScheduleQueryHandler(
        IServiceScheduleRepository serviceScheduleRepository,
        IValidator<GetAllServiceScheduleQuery> validator,
        IAppLogger<GetAllServiceScheduleQueryHandler> logger,
        IMapper mapper)
    {
        _serviceScheduleRepository = serviceScheduleRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PagedResult<GetAllServiceScheduleDTO>> Handle(GetAllServiceScheduleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllServiceScheduleQuery");
        // Validate the request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"GetAllServiceSchedule - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Fetch paged service schedules from the repository
        var result = await _serviceScheduleRepository.GetAllServiceSchedulesPagedAsync(request.Parameters);

        // Map schedules to DTOs and include ServiceTasks
        var scheduleDTOs = result.Items.Select(schedule =>
        {
            var dto = _mapper.Map<GetAllServiceScheduleDTO>(schedule);
            dto.ServiceTasks = schedule.XrefServiceScheduleServiceTasks
                .Select(xref => _mapper.Map<GetAllServiceTaskDTO>(xref.ServiceTask))
                .ToList();
            return dto;
        }).ToList();

        var pagedResult = new PagedResult<GetAllServiceScheduleDTO>
        {
            Items = scheduleDTOs,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} service schedules for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }
}
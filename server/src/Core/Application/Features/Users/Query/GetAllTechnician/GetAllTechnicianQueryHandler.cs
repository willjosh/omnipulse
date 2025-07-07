using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.Users.Query.GetAllTechnician;

public class GetAllTechnicianQueryHandler : IRequestHandler<GetAllTechnicianQuery, PagedResult<GetAllTechnicianDTO>>
{

    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllTechnicianQueryHandler> _logger;
    private readonly IValidator<GetAllTechnicianQuery> _validator;

    public GetAllTechnicianQueryHandler(IUserRepository userRepository, IMapper mapper, IAppLogger<GetAllTechnicianQueryHandler> logger, IValidator<GetAllTechnicianQuery> validator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<PagedResult<GetAllTechnicianDTO>> Handle(GetAllTechnicianQuery request, CancellationToken cancellationToken)
    {
        // validate the request
        _logger.LogInformation("Handling GetAllTechnicianQuery");
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"GetAllTechnician - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // get all technicians from the repository
        var result = await _userRepository.GetAllTechnicianPagedAsync(request.Parameters);

        // map the technicians to DTOs
        var technicianDTOs = _mapper.Map<List<GetAllTechnicianDTO>>(result.Items);

        var pagedResult = new PagedResult<GetAllTechnicianDTO>
        {
            Items = technicianDTOs,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} technicians for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }
}
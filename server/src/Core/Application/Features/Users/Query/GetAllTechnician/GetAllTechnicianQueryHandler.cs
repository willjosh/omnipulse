using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
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

    Task<PagedResult<GetAllTechnicianDTO>> IRequestHandler<GetAllTechnicianQuery, PagedResult<GetAllTechnicianDTO>>.Handle(GetAllTechnicianQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
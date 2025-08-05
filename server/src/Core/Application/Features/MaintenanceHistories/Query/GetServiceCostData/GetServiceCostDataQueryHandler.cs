using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;

using AutoMapper;

using MediatR;

namespace Application.Features.MaintenanceHistories.Query.GetServiceCostData;

public class GetServiceCostDataQueryHandler : IRequestHandler<GetServiceCostDataQuery, GetServiceCostDataDTO>
{
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetServiceCostDataQueryHandler> _logger;
    private readonly IMaintenanceHistoryRepository _maintenanceHistoryRepository;

    public GetServiceCostDataQueryHandler(IMapper mapper, IAppLogger<GetServiceCostDataQueryHandler> logger, IMaintenanceHistoryRepository maintenanceHistoryRepository)
    {
        _mapper = mapper;
        _logger = logger;
        _maintenanceHistoryRepository = maintenanceHistoryRepository;
    }

    public Task<GetServiceCostDataDTO> Handle(GetServiceCostDataQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
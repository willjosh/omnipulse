using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;

using AutoMapper;

using MediatR;

namespace Application.Features.InventoryItemLocations.Query.GetAllInventoryItemLocation;

public class GetAllInventoryItemLocationQueryHandler : IRequestHandler<GetAllInventoryItemLocationQuery, List<GetAllInventoryItemLocationDTO>>
{
    private readonly IInventoryItemLocationRepository _repository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllInventoryItemLocationQueryHandler> _logger;

    public GetAllInventoryItemLocationQueryHandler(
        IInventoryItemLocationRepository repository,
        IMapper mapper,
        IAppLogger<GetAllInventoryItemLocationQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<GetAllInventoryItemLocationDTO>> Handle(GetAllInventoryItemLocationQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllInventoryItemLocationQuery (no pagination)");
        var allEntities = await _repository.GetAllAsync();
        var allDTOs = _mapper.Map<List<GetAllInventoryItemLocationDTO>>(allEntities);
        return allDTOs;
    }
}
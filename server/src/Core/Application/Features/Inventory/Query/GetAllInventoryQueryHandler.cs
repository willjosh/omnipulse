using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.Inventory.Query;

public class GetAllInventoryQueryHandler : IRequestHandler<GetAllInventoryQuery, PagedResult<GetAllInventoryDTO>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllInventoryQueryHandler> _logger;
    private readonly IValidator<GetAllInventoryQuery> _validator;
    public GetAllInventoryQueryHandler(IInventoryRepository inventoryRepository, IMapper mapper, IAppLogger<GetAllInventoryQueryHandler> logger, IValidator<GetAllInventoryQuery> validator)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public Task<PagedResult<GetAllInventoryDTO>> Handle(GetAllInventoryQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
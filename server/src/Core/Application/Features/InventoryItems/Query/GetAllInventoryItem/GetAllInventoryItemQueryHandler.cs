using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.InventoryItems.Query.GetAllInventoryItem;

public class GetAllInventoryItemQueryHandler : IRequestHandler<GetAllInventoryItemQuery, PagedResult<GetAllInventoryItemDTO>>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllInventoryItemQueryHandler> _logger;
    private readonly IValidator<GetAllInventoryItemQuery> _validator;

    public GetAllInventoryItemQueryHandler(IInventoryItemRepository inventoryItemRepository, IMapper mapper, IAppLogger<GetAllInventoryItemQueryHandler> logger, IValidator<GetAllInventoryItemQuery> validator)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public Task<PagedResult<GetAllInventoryItemDTO>> Handle(GetAllInventoryItemQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
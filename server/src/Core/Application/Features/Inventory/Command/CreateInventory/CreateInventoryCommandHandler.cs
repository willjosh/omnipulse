using System;

using Application.Contracts.Persistence;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.Inventory.Command.CreateInventory;

public class CreateInventoryCommandHandler : IRequestHandler<CreateInventoryCommand, int>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IInventoryItemLocationRepository _inventoryItemLocationRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateInventoryCommand> _validator;

    public CreateInventoryCommandHandler(
        IInventoryRepository inventoryRepository,
        IInventoryItemRepository inventoryItemRepository,
        IInventoryItemLocationRepository inventoryItemLocationRepository,
        IMapper mapper,
        IValidator<CreateInventoryCommand> validator
    )
    {
        _inventoryRepository = inventoryRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _inventoryItemLocationRepository = inventoryItemLocationRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public Task<int> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
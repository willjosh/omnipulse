using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.InventoryItems.Command.CreateInventoryItem;

public class CreateInventoryItemCommandHandler : IRequestHandler<CreateInventoryItemCommand, int>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IValidator<CreateInventoryItemCommand> _validator;
    private readonly IAppLogger<CreateInventoryItemCommandHandler> _logger;
    private readonly IMapper _mapper;

    public CreateInventoryItemCommandHandler(
        IInventoryItemRepository inventoryItemRepository,
        IInventoryRepository inventoryRepository,
        IValidator<CreateInventoryItemCommand> validator,
        IAppLogger<CreateInventoryItemCommandHandler> logger,
        IMapper mapper)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<int> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        // validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"CreateInventoryItemCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // map request to inventory item domain entity
        var inventoryItem = _mapper.Map<InventoryItem>(request);

        // validate business rules
        await ValidateBusinessRuleAsync(inventoryItem);

        // add new inventory item
        var newInventoryItem = await _inventoryItemRepository.AddAsync(inventoryItem);

        // add new inventory 
        var inventory = Domain.Entities.Inventory.CreateDefaultInventory(newInventoryItem.ID);
        _inventoryRepository.AddAsync(inventory);


        // save changes
        await _inventoryItemRepository.SaveChangesAsync();

        // return inventory item ID
        return newInventoryItem.ID;
    }

    private async Task ValidateBusinessRuleAsync(InventoryItem inventoryItem)
    {
        // check for duplicate Item Number in the db
        if (!await _inventoryItemRepository.IsItemNumberUniqueAsync(inventoryItem.ItemNumber))
        {
            var errorMessage = $"Item number already exists: {inventoryItem.ItemNumber}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(typeof(InventoryItem).ToString(), "ItemNumber", inventoryItem.ItemNumber);
        }

        // check for duplicate Universal Product Code if provided
        if (!string.IsNullOrWhiteSpace(inventoryItem.UniversalProductCode) &&
            !await _inventoryItemRepository.IsUniversalProductCodeUniqueAsync(inventoryItem.UniversalProductCode))
        {
            var errorMessage = $"Universal Product Code already exists: {inventoryItem.UniversalProductCode}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(typeof(InventoryItem).ToString(), "UniversalProductCode", inventoryItem.UniversalProductCode);
        }

        // check for duplicate Manufacturer Part Number if both manufacturer and part number are provided
        if (!string.IsNullOrWhiteSpace(inventoryItem.Manufacturer) &&
            !string.IsNullOrWhiteSpace(inventoryItem.ManufacturerPartNumber) &&
            !await _inventoryItemRepository.IsManufacturerPartNumberUniqueAsync(inventoryItem.Manufacturer, inventoryItem.ManufacturerPartNumber))
        {
            var errorMessage = $"Manufacturer part number already exists for {inventoryItem.Manufacturer}: {inventoryItem.ManufacturerPartNumber}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(typeof(InventoryItem).ToString(), "ManufacturerPartNumber", $"{inventoryItem.Manufacturer}-{inventoryItem.ManufacturerPartNumber}");
        }
    }
}
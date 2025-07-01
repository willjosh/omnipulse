using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.InventoryItems.Command.UpdateInventoryItem;

public class UpdateInventoryItemCommandHandler : IRequestHandler<UpdateInventoryItemCommand, int>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IValidator<UpdateInventoryItemCommand> _validator;
    private readonly IAppLogger<UpdateInventoryItemCommandHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateInventoryItemCommandHandler(
        IInventoryItemRepository inventoryItemRepository,
        IValidator<UpdateInventoryItemCommand> validator,
        IAppLogger<UpdateInventoryItemCommandHandler> logger,
        IMapper mapper)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(UpdateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"UpdateInventoryItemCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Check if inventory item exists
        var existingInventoryItem = await _inventoryItemRepository.GetByIdAsync(request.InventoryItemID);
        if (existingInventoryItem == null)
        {
            var errorMessage = $"InventoryItem ID not found: {request.InventoryItemID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(InventoryItem).ToString(), "InventoryItemID", request.InventoryItemID.ToString());
        }

        // Validate business rules
        await ValidateBusinessRulesAsync(request, existingInventoryItem);

        // Map request to inventory item entity (this will update the existing inventory item properties)
        _mapper.Map(request, existingInventoryItem);

        // Update inventory item
        _inventoryItemRepository.Update(existingInventoryItem);

        // Save changes
        await _inventoryItemRepository.SaveChangesAsync();

        _logger.LogInformation($"Successfully updated inventory item with ID: {request.InventoryItemID}");

        // Return inventory item ID
        return existingInventoryItem.ID;
    }

    private async Task ValidateBusinessRulesAsync(UpdateInventoryItemCommand request, InventoryItem existingInventoryItem)
    {
        // Check for duplicate Item Number only if Item Number is being changed
        if (request.ItemNumber != existingInventoryItem.ItemNumber &&
            !await _inventoryItemRepository.IsItemNumberUniqueAsync(request.ItemNumber))
        {
            var errorMessage = $"Item number already exists: {request.ItemNumber}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(typeof(InventoryItem).ToString(), "ItemNumber", request.ItemNumber);
        }

        // Check for duplicate Universal Product Code only if UPC is being changed and is not null
        if (request.UniversalProductCode != existingInventoryItem.UniversalProductCode &&
            !string.IsNullOrWhiteSpace(request.UniversalProductCode) &&
            !await _inventoryItemRepository.IsUniversalProductCodeUniqueAsync(request.UniversalProductCode))
        {
            var errorMessage = $"Universal Product Code already exists: {request.UniversalProductCode}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(typeof(InventoryItem).ToString(), "UniversalProductCode", request.UniversalProductCode);
        }

        // Check for duplicate Manufacturer Part Number only if either manufacturer or part number is being changed and both are provided
        var manufacturerChanged = request.Manufacturer != existingInventoryItem.Manufacturer;
        var partNumberChanged = request.ManufacturerPartNumber != existingInventoryItem.ManufacturerPartNumber;

        if ((manufacturerChanged || partNumberChanged) &&
            !string.IsNullOrWhiteSpace(request.Manufacturer) &&
            !string.IsNullOrWhiteSpace(request.ManufacturerPartNumber) &&
            !await _inventoryItemRepository.IsManufacturerPartNumberUniqueAsync(request.Manufacturer, request.ManufacturerPartNumber))
        {
            var errorMessage = $"Manufacturer part number already exists for {request.Manufacturer}: {request.ManufacturerPartNumber}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(typeof(InventoryItem).ToString(), "ManufacturerPartNumber", $"{request.Manufacturer}-{request.ManufacturerPartNumber}");
        }
    }
}
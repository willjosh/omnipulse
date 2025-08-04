using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Inventory.Command.UpdateInventory;

public class UpdateInventoryCommandHandler : IRequestHandler<UpdateInventoryCommand, int>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
    private readonly IValidator<UpdateInventoryCommand> _validator;
    private readonly IMapper _mapper;
    private readonly IAppLogger<UpdateInventoryCommandHandler> _logger;

    public UpdateInventoryCommandHandler(IInventoryRepository inventoryRepository, IInventoryTransactionRepository inventoryTransactionRepository, IMapper mapper, IAppLogger<UpdateInventoryCommandHandler> logger, IValidator<UpdateInventoryCommand> validator)
    {
        _inventoryRepository = inventoryRepository;
        _inventoryTransactionRepository = inventoryTransactionRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<int> Handle(UpdateInventoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(UpdateInventoryCommand)} for InventoryID: {request.InventoryID}");

        // validate the request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(UpdateInventoryCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // check if inventory exists
        var existingInventory = await _inventoryRepository.GetByIdAsync(request.InventoryID);
        if (existingInventory == null)
        {
            var errorMessage = $"{nameof(Inventory)} with ID {request.InventoryID} not found";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(Inventory), nameof(Domain.Entities.Inventory.ID), request.InventoryID.ToString());
        }

        // STORE OLD VALUES BEFORE MAPPING
        var oldQuantity = existingInventory.QuantityOnHand;
        var oldUnitCost = existingInventory.UnitCost;

        // map request to entity (update properties)
        _mapper.Map(request, existingInventory);

        // Update last restocked date only if quantity changed
        if (oldQuantity != request.QuantityOnHand)
        {
            existingInventory.LastRestockedDate = DateTime.UtcNow; // Use UtcNow for consistency
        }

        // decide the needs reorder status
        existingInventory.DecideReorderStatus();

        // Calculate actual quantity change
        var quantityChange = request.QuantityOnHand - oldQuantity;

        // Only create transaction if there's an actual change
        if (quantityChange != 0 || oldUnitCost != request.UnitCost)
        {
            // decide transaction type with CORRECT old vs new quantities
            var transactionType = InventoryTransaction.DecideTransactionType(
                request.IsAdjustment,
                request.QuantityOnHand,  // NEW quantity
                oldQuantity              // OLD quantity (stored before mapping)
            );

            // create the inventory transaction
            var inventoryTransaction = new InventoryTransaction
            {
                ID = 0, // Auto-generated
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                InventoryID = existingInventory.ID,
                TransactionType = transactionType,
                Quantity = Math.Abs(quantityChange), // Store absolute quantity change
                UnitCost = request.UnitCost,
                TotalCost = Math.Abs(quantityChange) * request.UnitCost,
                PerformedByUserID = request.PerformedByUserID,
                Inventory = existingInventory,
                User = null!
            };

            await _inventoryTransactionRepository.AddAsync(inventoryTransaction);
            await _inventoryTransactionRepository.SaveChangesAsync();

            _logger.LogInformation($"Created inventory transaction for InventoryID: {request.InventoryID}, Type: {transactionType}, Quantity Change: {quantityChange}");
        }

        // update the inventory
        _inventoryRepository.Update(existingInventory);
        await _inventoryRepository.SaveChangesAsync();

        _logger.LogInformation($"Successfully updated inventory with ID: {existingInventory.ID}");

        return existingInventory.ID;
    }
}
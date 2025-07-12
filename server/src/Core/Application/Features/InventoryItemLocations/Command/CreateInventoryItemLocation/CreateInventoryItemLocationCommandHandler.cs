using System;
using System.Linq;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.InventoryItemLocations.Command.CreateInventoryItemLocation;

public class CreateInventoryItemLocationCommandHandler : IRequestHandler<CreateInventoryItemLocationCommand, int>
{
    private readonly IInventoryItemLocationRepository _inventoryItemLocationRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateInventoryItemLocationCommandHandler> _logger;
    private readonly IValidator<CreateInventoryItemLocationCommand> _validator;

    public CreateInventoryItemLocationCommandHandler(IInventoryItemLocationRepository inventoryItemLocationRepository, IMapper mapper, IAppLogger<CreateInventoryItemLocationCommandHandler> logger, IValidator<CreateInventoryItemLocationCommand> validator)
    {
        _inventoryItemLocationRepository = inventoryItemLocationRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<int> Handle(CreateInventoryItemLocationCommand request, CancellationToken cancellationToken)
    {
        // validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"CreateInventoryItemLocationCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // map request to inventory item location domain entity
        var inventoryItemLocation = _mapper.Map<InventoryItemLocation>(request);

        // add new inventory item location
        var newInventoryItemLocation = await _inventoryItemLocationRepository.AddAsync(inventoryItemLocation);

        // save changes
        await _inventoryItemLocationRepository.SaveChangesAsync();

        // return inventory item location ID
        return newInventoryItemLocation.ID;
    }
}
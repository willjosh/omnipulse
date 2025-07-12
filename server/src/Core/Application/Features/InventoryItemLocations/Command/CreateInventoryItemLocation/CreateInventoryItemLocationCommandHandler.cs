using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;

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

    public Task<int> Handle(CreateInventoryItemLocationCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.InspectionFormItems.Command.CreateInspectionFormItem;

/// <summary>
/// Handles <see cref="CreateInspectionFormItemCommand"/>
/// </summary>
public sealed class CreateInspectionFormItemCommandHandler : IRequestHandler<CreateInspectionFormItemCommand, int>
{
    private readonly IInspectionFormItemRepository _inspectionFormItemRepository;
    private readonly IInspectionFormRepository _inspectionFormRepository;
    private readonly IValidator<CreateInspectionFormItemCommand> _validator;
    private readonly IAppLogger<CreateInspectionFormItemCommandHandler> _logger;
    private readonly IMapper _mapper;

    public CreateInspectionFormItemCommandHandler(
        IInspectionFormItemRepository inspectionFormItemRepository,
        IInspectionFormRepository inspectionFormRepository,
        IValidator<CreateInspectionFormItemCommand> validator,
        IAppLogger<CreateInspectionFormItemCommandHandler> logger,
        IMapper mapper)
    {
        _inspectionFormItemRepository = inspectionFormItemRepository;
        _inspectionFormRepository = inspectionFormRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateInspectionFormItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(CreateInspectionFormItemCommand)}");

        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(CreateInspectionFormItemCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Validate business rules
        await ValidateBusinessRuleAsync(request);

        // Map request to inspection form item domain entity
        var inspectionFormItem = _mapper.Map<InspectionFormItem>(request);

        // Add new inspection form item
        var newInspectionFormItem = await _inspectionFormItemRepository.AddAsync(inspectionFormItem);

        // Save changes
        await _inspectionFormItemRepository.SaveChangesAsync();

        _logger.LogInformation($"Inspection form item '{newInspectionFormItem.ItemLabel}' created successfully with ID: {newInspectionFormItem.ID}");

        // Return inspection form item ID
        return newInspectionFormItem.ID;
    }

    private async Task ValidateBusinessRuleAsync(CreateInspectionFormItemCommand request)
    {
        // Check if the inspection form exists
        var inspectionForm = await _inspectionFormRepository.GetByIdAsync(request.InspectionFormID);
        if (inspectionForm == null)
        {
            var errorMessage = $"{nameof(InspectionForm)} with ID {request.InspectionFormID} not found.";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(InspectionForm), nameof(InspectionForm.ID), request.InspectionFormID.ToString());
        }

        // Check if the inspection form is active
        if (!inspectionForm.IsActive)
        {
            var errorMessage = $"Cannot add items to inactive {nameof(InspectionForm)} with ID {request.InspectionFormID}.";
            _logger.LogError(errorMessage);
            throw new BadRequestException(errorMessage);
        }
    }
}
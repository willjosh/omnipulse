using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.InspectionFormItems.Command.UpdateInspectionFormItem;

/// <summary>
/// Handles <see cref="UpdateInspectionFormItemCommand"/>
/// </summary>
public sealed class UpdateInspectionFormItemCommandHandler : IRequestHandler<UpdateInspectionFormItemCommand, int>
{
    private readonly IInspectionFormItemRepository _inspectionFormItemRepository;
    private readonly IInspectionFormRepository _inspectionFormRepository;
    private readonly IValidator<UpdateInspectionFormItemCommand> _validator;
    private readonly IAppLogger<UpdateInspectionFormItemCommandHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateInspectionFormItemCommandHandler(
        IInspectionFormItemRepository inspectionFormItemRepository,
        IInspectionFormRepository inspectionFormRepository,
        IValidator<UpdateInspectionFormItemCommand> validator,
        IAppLogger<UpdateInspectionFormItemCommandHandler> logger,
        IMapper mapper)
    {
        _inspectionFormItemRepository = inspectionFormItemRepository;
        _inspectionFormRepository = inspectionFormRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(UpdateInspectionFormItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(UpdateInspectionFormItemCommand)} for ID: {request.InspectionFormItemID}");

        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(UpdateInspectionFormItemCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Check if inspection form item exists
        var existingItem = await _inspectionFormItemRepository.GetByIdAsync(request.InspectionFormItemID);
        if (existingItem == null)
        {
            var errorMessage = $"{nameof(InspectionFormItem)} {nameof(InspectionFormItem.ID)} not found: {request.InspectionFormItemID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(InspectionFormItem), nameof(InspectionFormItem.ID), request.InspectionFormItemID.ToString());
        }

        // Validate business rules
        await ValidateBusinessRuleAsync(existingItem);

        // Map request to entity (update properties)
        _mapper.Map(request, existingItem);

        // Update & Save
        _inspectionFormItemRepository.Update(existingItem);
        await _inspectionFormItemRepository.SaveChangesAsync();

        _logger.LogInformation($"Successfully updated {nameof(InspectionFormItem)} '{existingItem.ItemLabel}' with ID: {request.InspectionFormItemID}");

        // Return ID
        return existingItem.ID;
    }

    private async Task ValidateBusinessRuleAsync(InspectionFormItem existingItem)
    {
        // Verify the current inspection form is active
        var currentInspectionForm = await _inspectionFormRepository.GetByIdAsync(existingItem.InspectionFormID);
        if (currentInspectionForm != null && !currentInspectionForm.IsActive)
        {
            var errorMessage = $"Cannot update items in inactive {nameof(InspectionForm)} with ID {existingItem.InspectionFormID}.";
            _logger.LogError(errorMessage);
            throw new BadRequestException(errorMessage);
        }
    }
}
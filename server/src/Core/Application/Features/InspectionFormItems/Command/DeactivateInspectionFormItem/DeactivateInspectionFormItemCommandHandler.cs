using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.InspectionFormItems.Command.DeactivateInspectionFormItem;

public class DeactivateInspectionFormItemCommandHandler : IRequestHandler<DeactivateInspectionFormItemCommand, int>
{
    private readonly IInspectionFormItemRepository _inspectionFormItemRepository;
    private readonly IValidator<DeactivateInspectionFormItemCommand> _validator;
    private readonly IAppLogger<DeactivateInspectionFormItemCommandHandler> _logger;

    public DeactivateInspectionFormItemCommandHandler(
        IInspectionFormItemRepository inspectionFormItemRepository,
        IValidator<DeactivateInspectionFormItemCommand> validator,
        IAppLogger<DeactivateInspectionFormItemCommandHandler> logger)
    {
        _inspectionFormItemRepository = inspectionFormItemRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<int> Handle(DeactivateInspectionFormItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(DeactivateInspectionFormItemCommand)} for ID: {request.InspectionFormItemID}");

        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(DeactivateInspectionFormItemCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Get InspectionFormItem by ID
        var targetInspectionFormItem = await _inspectionFormItemRepository.GetByIdAsync(request.InspectionFormItemID);
        if (targetInspectionFormItem == null)
        {
            var errorMessage = $"{nameof(InspectionFormItem)} with {nameof(InspectionFormItem.ID)} {request.InspectionFormItemID} not found.";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(InspectionFormItem), nameof(InspectionFormItem.ID), request.InspectionFormItemID.ToString());
        }

        // Validate business rules before soft deletion
        ValidateBusinessRuleAsync(targetInspectionFormItem);

        // Soft delete by setting IsActive to false
        targetInspectionFormItem.IsActive = false;
        _inspectionFormItemRepository.Update(targetInspectionFormItem);

        // Save changes
        await _inspectionFormItemRepository.SaveChangesAsync();

        _logger.LogInformation($"Successfully soft deleted (deactivated) {nameof(InspectionFormItem)} with ID: {request.InspectionFormItemID}");

        // Return the soft-deleted InspectionFormItemID
        return request.InspectionFormItemID;
    }

    private void ValidateBusinessRuleAsync(InspectionFormItem inspectionFormItem)
    {
        // Check if the item is already inactive (already deactivated)
        if (!inspectionFormItem.IsActive)
        {
            var errorMessage = $"Cannot deactivate {nameof(InspectionFormItem)} with ID {inspectionFormItem.ID} because it is already inactive (already deactivated).";
            _logger.LogError(errorMessage);
            throw new BadRequestException(errorMessage);
        }

        // Note: We allow deactivation even if there are associated inspection results
        // This preserves inspection history while making the item unavailable for new inspections
    }
}
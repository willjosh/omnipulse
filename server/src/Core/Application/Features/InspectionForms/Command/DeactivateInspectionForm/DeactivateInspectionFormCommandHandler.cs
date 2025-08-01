using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.InspectionForms.Command.DeactivateInspectionForm;

public class DeactivateInspectionFormCommandHandler : IRequestHandler<DeactivateInspectionFormCommand, int>
{
    private readonly IInspectionFormRepository _inspectionFormRepository;
    private readonly IValidator<DeactivateInspectionFormCommand> _validator;
    private readonly IAppLogger<DeactivateInspectionFormCommandHandler> _logger;

    public DeactivateInspectionFormCommandHandler(
        IInspectionFormRepository inspectionFormRepository,
        IValidator<DeactivateInspectionFormCommand> validator,
        IAppLogger<DeactivateInspectionFormCommandHandler> logger)
    {
        _inspectionFormRepository = inspectionFormRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<int> Handle(DeactivateInspectionFormCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(DeactivateInspectionFormCommand)} for ID: {request.InspectionFormID}");

        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(DeactivateInspectionFormCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Get InspectionForm by ID
        var targetInspectionForm = await _inspectionFormRepository.GetByIdAsync(request.InspectionFormID);
        if (targetInspectionForm == null)
        {
            var errorMessage = $"{nameof(InspectionForm)} with {nameof(InspectionForm.ID)} {request.InspectionFormID} not found.";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(InspectionForm), nameof(InspectionForm.ID), request.InspectionFormID.ToString());
        }

        // Validate business rules before soft deletion
        ValidateBusinessRuleAsync(targetInspectionForm);

        // Soft delete by setting IsActive to false (preserves inspection history)
        targetInspectionForm.IsActive = false;
        _inspectionFormRepository.Update(targetInspectionForm);

        // Save changes
        await _inspectionFormRepository.SaveChangesAsync();

        _logger.LogInformation($"Successfully soft deleted (deactivated) {nameof(InspectionForm)} with ID: {request.InspectionFormID}");

        // Return the deleted InspectionFormID
        return request.InspectionFormID;
    }

    private void ValidateBusinessRuleAsync(InspectionForm inspectionForm)
    {
        // Check if the form is already inactive (already deactivated)
        if (!inspectionForm.IsActive)
        {
            var errorMessage = $"Cannot deactivate {nameof(InspectionForm)} with ID {inspectionForm.ID} because it is already inactive (already deactivated).";
            _logger.LogError(errorMessage);
            throw new BadRequestException(errorMessage);
        }

        // Note: We allow deactivation even if there are associated inspections
        // This preserves inspection history while making the form unavailable for new inspections
    }
}
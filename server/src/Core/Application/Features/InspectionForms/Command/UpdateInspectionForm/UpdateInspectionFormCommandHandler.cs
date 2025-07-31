using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.InspectionForms.Command.UpdateInspectionForm;

public class UpdateInspectionFormCommandHandler : IRequestHandler<UpdateInspectionFormCommand, int>
{
    private readonly IInspectionFormRepository _inspectionFormRepository;
    private readonly IValidator<UpdateInspectionFormCommand> _validator;
    private readonly IAppLogger<UpdateInspectionFormCommandHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateInspectionFormCommandHandler(
        IInspectionFormRepository inspectionFormRepository,
        IValidator<UpdateInspectionFormCommand> validator,
        IAppLogger<UpdateInspectionFormCommandHandler> logger,
        IMapper mapper)
    {
        _inspectionFormRepository = inspectionFormRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(UpdateInspectionFormCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(UpdateInspectionFormCommand)}");

        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(UpdateInspectionFormCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Check if inspection form exists
        var existingForm = await _inspectionFormRepository.GetByIdAsync(request.InspectionFormID);
        if (existingForm == null)
        {
            var errorMessage = $"{nameof(InspectionForm)} {nameof(InspectionForm.ID)} not found: {request.InspectionFormID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(InspectionForm), nameof(InspectionForm.ID), request.InspectionFormID.ToString());
        }

        // Validate business rules
        await ValidateBusinessRuleAsync(existingForm, request);

        // Map request to entity (update properties)
        _mapper.Map(request, existingForm);

        // Update & Save
        _inspectionFormRepository.Update(existingForm);
        await _inspectionFormRepository.SaveChangesAsync();

        _logger.LogInformation($"Successfully updated {nameof(InspectionForm)} with {nameof(InspectionForm.ID)}: {request.InspectionFormID}");

        // Return ID
        return existingForm.ID;
    }

    private async Task ValidateBusinessRuleAsync(InspectionForm existingForm, UpdateInspectionFormCommand request)
    {
        // Check for duplicate title if changed
        if (!string.Equals(existingForm.Title, request.Title, StringComparison.Ordinal) &&
            !await _inspectionFormRepository.IsTitleUniqueAsync(request.Title, request.InspectionFormID))
        {
            var errorMessage = $"{nameof(InspectionForm)} {nameof(InspectionForm.Title)} already exists: {request.Title}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(nameof(InspectionForm), nameof(InspectionForm.Title), request.Title);
        }
    }
}
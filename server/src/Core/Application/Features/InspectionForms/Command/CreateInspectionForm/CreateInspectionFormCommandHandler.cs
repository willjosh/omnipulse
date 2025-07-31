using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.InspectionForms.Command.CreateInspectionForm;

public class CreateInspectionFormCommandHandler : IRequestHandler<CreateInspectionFormCommand, int>
{
    private readonly IInspectionFormRepository _inspectionFormRepository;
    private readonly IValidator<CreateInspectionFormCommand> _validator;
    private readonly IAppLogger<CreateInspectionFormCommandHandler> _logger;
    private readonly IMapper _mapper;

    public CreateInspectionFormCommandHandler(
        IInspectionFormRepository inspectionFormRepository,
        IValidator<CreateInspectionFormCommand> validator,
        IAppLogger<CreateInspectionFormCommandHandler> logger,
        IMapper mapper)
    {
        _inspectionFormRepository = inspectionFormRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateInspectionFormCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(CreateInspectionFormCommand)}");

        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(CreateInspectionFormCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Map request to inspection form domain entity
        var inspectionForm = _mapper.Map<InspectionForm>(request);

        // Validate business rules
        await ValidateBusinessRuleAsync(inspectionForm);

        // Add new inspection form
        var newInspectionForm = await _inspectionFormRepository.AddAsync(inspectionForm);

        // Save changes
        await _inspectionFormRepository.SaveChangesAsync();

        _logger.LogInformation($"Inspection form created successfully with ID: {newInspectionForm.ID}");
        return newInspectionForm.ID;
    }

    private async Task ValidateBusinessRuleAsync(InspectionForm inspectionForm)
    {
        // Check for duplicate title in DB
        if (!await _inspectionFormRepository.IsTitleUniqueAsync(inspectionForm.Title))
        {
            var errorMessage = $"{nameof(InspectionForm.Title)} already exists: {inspectionForm.Title}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(typeof(InspectionForm).ToString(), nameof(InspectionForm.Title), inspectionForm.Title);
        }
    }
}
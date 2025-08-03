using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using MediatR;

namespace Application.Features.Inspections.Command.CreateInspection;

public class CreateInspectionCommandHandler : IRequestHandler<CreateInspectionCommand, int>
{
    private readonly IInspectionRepository _inspectionRepository;
    private readonly IInspectionFormRepository _inspectionFormRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IValidator<CreateInspectionCommand> _validator;
    private readonly IAppLogger<CreateInspectionCommandHandler> _logger;
    private readonly IMapper _mapper;

    public CreateInspectionCommandHandler(
        IInspectionRepository inspectionRepository,
        IInspectionFormRepository inspectionFormRepository,
        IVehicleRepository vehicleRepository,
        IUserRepository userRepository,
        IIssueRepository issueRepository,
        IValidator<CreateInspectionCommand> validator,
        IAppLogger<CreateInspectionCommandHandler> logger,
        IMapper mapper)
    {
        _inspectionRepository = inspectionRepository;
        _inspectionFormRepository = inspectionFormRepository;
        _vehicleRepository = vehicleRepository;
        _userRepository = userRepository;
        _issueRepository = issueRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateInspectionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(CreateInspectionCommand)}");

        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(CreateInspectionCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Validate business rules and get related entities
        var (inspectionForm, vehicle, technician) = await ValidateBusinessRulesAsync(request);

        // Map request to inspection domain entity
        var inspection = _mapper.Map<Inspection>(request);

        // Set snapshot fields
        inspection.SnapshotFormTitle = inspectionForm.Title;
        inspection.SnapshotFormDescription = inspectionForm.Description;

        // Create inspection pass/fail items with snapshot data
        var inspectionPassFailItems = new List<InspectionPassFailItem>();
        foreach (var itemRequest in request.InspectionItems)
        {
            var formItem = inspectionForm.InspectionFormItems
                .FirstOrDefault(fi => fi.ID == itemRequest.InspectionFormItemID);

            if (formItem == null)
            {
                var errorMessage = $"InspectionFormItem with ID {itemRequest.InspectionFormItemID} not found in the specified inspection form";
                _logger.LogError(errorMessage);
                throw new EntityNotFoundException(typeof(InspectionFormItem).ToString(), "ID", itemRequest.InspectionFormItemID.ToString());
            }

            var passFailItem = new InspectionPassFailItem
            {
                InspectionID = 0, // Will be set after inspection is created
                InspectionFormItemID = itemRequest.InspectionFormItemID,
                Passed = itemRequest.Passed,
                Comment = itemRequest.Comment,
                Inspection = inspection,
                InspectionFormItem = formItem,
                // Set snapshot properties from the form item
                SnapshotItemLabel = formItem.ItemLabel,
                SnapshotItemDescription = formItem.ItemDescription,
                SnapshotItemInstructions = formItem.ItemInstructions,
                SnapshotIsRequired = formItem.IsRequired,
                SnapshotInspectionFormItemTypeEnum = formItem.InspectionFormItemTypeEnum
            };

            inspectionPassFailItems.Add(passFailItem);
        }

        inspection.InspectionPassFailItems = inspectionPassFailItems;

        // Add new inspection
        var newInspection = await _inspectionRepository.AddAsync(inspection);

        // Save changes
        await _inspectionRepository.SaveChangesAsync();

        // Create issues for failed inspection items
        await CreateIssuesForFailedItemsAsync(newInspection);

        _logger.LogInformation($"Inspection created successfully with ID: {newInspection.ID}");
        return newInspection.ID;
    }

    private async Task<(InspectionForm inspectionForm, Vehicle vehicle, User technician)> ValidateBusinessRulesAsync(CreateInspectionCommand request)
    {
        // Check if inspection form exists and is active
        var inspectionForm = await _inspectionFormRepository.GetByIdAsync(request.InspectionFormID);
        if (inspectionForm == null)
        {
            var errorMessage = $"InspectionForm not found: {request.InspectionFormID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(InspectionForm).ToString(), "ID", request.InspectionFormID.ToString());
        }

        if (!inspectionForm.IsActive)
        {
            var errorMessage = $"InspectionForm is not active: {request.InspectionFormID}";
            _logger.LogError(errorMessage);
            throw new BadRequestException($"Cannot create inspection using inactive inspection form: {inspectionForm.Title}");
        }

        // Load inspection form items to validate against
        var inspectionFormWithItems = await _inspectionFormRepository.GetInspectionFormWithItemsAsync(request.InspectionFormID);
        if (inspectionFormWithItems?.InspectionFormItems == null || inspectionFormWithItems.InspectionFormItems.Count == 0)
        {
            var errorMessage = $"InspectionForm has no active items: {request.InspectionFormID}";
            _logger.LogError(errorMessage);
            throw new BadRequestException($"Cannot create inspection for form with no active items: {inspectionForm.Title}");
        }

        // Check if vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleID);
        if (vehicle == null)
        {
            var errorMessage = $"Vehicle not found: {request.VehicleID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(Vehicle).ToString(), "ID", request.VehicleID.ToString());
        }

        // Check if technician exists
        var technician = await _userRepository.GetTechnicianByIdAsync(request.TechnicianID);
        if (technician == null)
        {
            var errorMessage = $"Technician not found: {request.TechnicianID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(User).ToString(), "ID", request.TechnicianID);
        }

        // Validate that all required form items have responses
        var activeFormItems = inspectionFormWithItems.InspectionFormItems.Where(fi => fi.IsActive).ToList();
        var requiredFormItemIds = activeFormItems.Where(fi => fi.IsRequired).Select(fi => fi.ID).ToHashSet();
        var providedFormItemIds = request.InspectionItems.Select(ii => ii.InspectionFormItemID).ToHashSet();

        var missingRequiredItems = requiredFormItemIds.Except(providedFormItemIds).ToList();
        if (missingRequiredItems.Count != 0)
        {
            var missingItemLabels = activeFormItems
                .Where(fi => missingRequiredItems.Contains(fi.ID))
                .Select(fi => fi.ItemLabel);
            var errorMessage = $"Missing responses for required inspection items: {string.Join(", ", missingItemLabels)}";
            _logger.LogError(errorMessage);
            throw new BadRequestException(errorMessage);
        }

        // Validate that all provided responses correspond to active form items
        var invalidFormItemIds = providedFormItemIds.Except(activeFormItems.Select(fi => fi.ID)).ToList();
        if (invalidFormItemIds.Count != 0)
        {
            var errorMessage = $"Invalid inspection form item IDs provided: {string.Join(", ", invalidFormItemIds)}";
            _logger.LogError(errorMessage);
            throw new BadRequestException(errorMessage);
        }

        return (inspectionFormWithItems, vehicle, technician);
    }

    private async Task CreateIssuesForFailedItemsAsync(Inspection inspection)
    {
        var failedItems = inspection.InspectionPassFailItems?.Where(item => !item.Passed).ToList();
        if (failedItems == null || failedItems.Count == 0)
        {
            _logger.LogInformation($"No failed inspection items found for inspection {inspection.ID}");
            return;
        }

        _logger.LogInformation($"Creating issues for {failedItems.Count} failed inspection items in inspection {inspection.ID}");

        // Get the vehicle and technician for the issue creation
        var vehicle = await _vehicleRepository.GetByIdAsync(inspection.VehicleID);
        var technician = await _userRepository.GetTechnicianByIdAsync(inspection.TechnicianID);

        if (vehicle == null)
        {
            _logger.LogError($"Vehicle not found for inspection {inspection.ID}");
            return;
        }

        if (technician == null)
        {
            _logger.LogError($"Technician not found for inspection {inspection.ID} with technician ID {inspection.TechnicianID}");
            return;
        }

        foreach (var failedItem in failedItems)
        {
            try
            {
                var currentTime = DateTime.UtcNow;

                var issue = new Issue
                {
                    ID = 0, // Will be set by DB
                    CreatedAt = currentTime,
                    UpdatedAt = currentTime,
                    VehicleID = inspection.VehicleID,
                    IssueNumber = 0, // Will be set by DB (same as ID)
                    ReportedByUserID = inspection.TechnicianID,
                    ReportedDate = currentTime,
                    Title = $"Inspection Failed: {failedItem.SnapshotItemLabel}",
                    Description = $"Inspection item '{failedItem.SnapshotItemLabel}' failed during inspection {inspection.ID}. " +
                                $"Item Description: {failedItem.SnapshotItemDescription}. " +
                                $"Technician Comment: {failedItem.Comment ?? "No comment provided"}.",
                    Category = DetermineIssueCategory(failedItem.SnapshotItemLabel, failedItem.SnapshotItemDescription),
                    PriorityLevel = DeterminePriorityLevel(failedItem.SnapshotIsRequired),
                    Status = IssueStatusEnum.OPEN,
                    InspectionID = inspection.ID,
                    Vehicle = vehicle,
                    ReportedByUser = technician,
                    IssueAttachments = [],
                    IssueAssignments = []
                };

                await _issueRepository.AddAsync(issue);
                _logger.LogInformation($"Created issue for failed inspection item {failedItem.InspectionFormItemID}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create issue for failed inspection item {failedItem.InspectionFormItemID}: {ex.Message}");
                // Continue with other failed items even if one fails
            }
        }

        // Save all issues
        await _issueRepository.SaveChangesAsync();
    }

    private IssueCategoryEnum DetermineIssueCategory(string itemLabel, string? itemDescription)
    {
        var labelAndDescription = $"{itemLabel} {itemDescription}".ToLowerInvariant();

        if (labelAndDescription.Contains("engine") || labelAndDescription.Contains("motor"))
            return IssueCategoryEnum.ENGINE;
        if (labelAndDescription.Contains("transmission") || labelAndDescription.Contains("gear"))
            return IssueCategoryEnum.TRANSMISSION;
        if (labelAndDescription.Contains("brake") || labelAndDescription.Contains("braking"))
            return IssueCategoryEnum.BRAKES;
        if (labelAndDescription.Contains("electrical") || labelAndDescription.Contains("battery") || labelAndDescription.Contains("light"))
            return IssueCategoryEnum.ELECTRICAL;
        if (labelAndDescription.Contains("body") || labelAndDescription.Contains("door") || labelAndDescription.Contains("window"))
            return IssueCategoryEnum.BODY;
        if (labelAndDescription.Contains("tire") || labelAndDescription.Contains("wheel"))
            return IssueCategoryEnum.TIRES;
        if (labelAndDescription.Contains("hvac") || labelAndDescription.Contains("air") || labelAndDescription.Contains("heating") || labelAndDescription.Contains("cooling"))
            return IssueCategoryEnum.HVAC;

        return IssueCategoryEnum.OTHER;
    }

    private PriorityLevelEnum DeterminePriorityLevel(bool isRequired)
    {
        // Required items that fail are high priority, optional items are medium priority
        return isRequired ? PriorityLevelEnum.HIGH : PriorityLevelEnum.MEDIUM;
    }
}
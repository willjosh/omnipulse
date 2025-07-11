using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.MaintenanceHistories.Command.CreateMaintenanceHistory;

public class CreateMaintenanceHistoryCommandHandler : IRequestHandler<CreateMaintenanceHistoryCommand, int>
{
    private readonly IMaintenanceHistoryRepository _maintenanceHistoryRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IServiceTaskRepository _serviceTaskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateMaintenanceHistoryCommandHandler> _logger;
    private readonly IValidator<CreateMaintenanceHistoryCommand> _validator;

    public CreateMaintenanceHistoryCommandHandler(
        IMaintenanceHistoryRepository maintenanceHistoryRepository,
        IVehicleRepository vehicleRepository,
        IWorkOrderRepository workOrderRepository,
        IServiceTaskRepository serviceTaskRepository,
        IUserRepository userRepository,
        IMapper mapper,
        IAppLogger<CreateMaintenanceHistoryCommandHandler> logger,
        IValidator<CreateMaintenanceHistoryCommand> validator)
    {
        _maintenanceHistoryRepository = maintenanceHistoryRepository;
        _vehicleRepository = vehicleRepository;
        _workOrderRepository = workOrderRepository;
        _serviceTaskRepository = serviceTaskRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<int> Handle(CreateMaintenanceHistoryCommand request, CancellationToken cancellationToken)
    {
        // validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"CreateMaintenanceHistoryCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // validate business rules
        await ValidateBusinessRules(request, cancellationToken);

        // map request to MaintenanceHistory domain entity
        var maintenanceHistory = _mapper.Map<MaintenanceHistory>(request);

        // add new maintenance history
        var newMaintenanceHistory = await _maintenanceHistoryRepository.AddAsync(maintenanceHistory);

        // save changes
        await _maintenanceHistoryRepository.SaveChangesAsync();

        // return MaintenanceHistoryID
        return newMaintenanceHistory.ID;
    }

    private async Task ValidateBusinessRules(CreateMaintenanceHistoryCommand request, CancellationToken cancellationToken)
    {
        if (!await _workOrderRepository.ExistsAsync(request.WorkOrderID))
        {
            var errorMessage = $"WorkOrder ID not found: {request.WorkOrderID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(MaintenanceHistory).ToString(), "WorkOrderID", request.WorkOrderID.ToString());
        }
    }
}
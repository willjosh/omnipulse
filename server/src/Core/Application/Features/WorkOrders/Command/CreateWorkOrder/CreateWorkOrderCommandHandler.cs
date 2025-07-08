using Application.Contracts.Logger;
using Application.Contracts.Persistence;

using AutoMapper;

using MediatR;

namespace Application.Features.WorkOrders.Command.CreateWorkOrder;

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, int>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IServiceReminderRepository _serviceReminderRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateWorkOrderCommandHandler> _logger;

    public CreateWorkOrderCommandHandler(IWorkOrderRepository workOrderRepository, IUserRepository userRepository, IVehicleRepository vehicleRepository, IServiceReminderRepository serviceReminder, IIssueRepository issueRepository, IAppLogger<CreateWorkOrderCommandHandler> logger, IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _serviceReminderRepository = serviceReminder;
        _issueRepository = issueRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<int> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
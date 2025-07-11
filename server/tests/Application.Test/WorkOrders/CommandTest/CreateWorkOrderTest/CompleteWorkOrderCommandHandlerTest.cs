using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.WorkOrders.Command.CompleteWorkOrder;
using Application.MappingProfiles;

using AutoMapper;

using Moq;

namespace Application.Test.WorkOrders.CommandTest.CreateWorkOrderTest;

public class CompleteWorkOrderCommandHandlerTest
{

    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository;
    private readonly Mock<IMaintenanceHistoryRepository> _mockMaintenanceHistoryRepository;
    private readonly Mock<IAppLogger<CompleteWorkOrderCommandHandler>> _mockLogger;
    private readonly CompleteWorkOrderCommandHandler _handler;

    public CompleteWorkOrderCommandHandlerTest()
    {
        _mockMaintenanceHistoryRepository = new();
        _mockMaintenanceHistoryRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WorkOrderMappingProfile>();
        });

        var mapper = config.CreateMapper();
    }
}
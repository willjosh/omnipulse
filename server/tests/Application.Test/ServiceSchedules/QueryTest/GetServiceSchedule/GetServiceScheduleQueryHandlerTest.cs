using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceSchedules.Query.GetServiceSchedule;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using Moq;

namespace Application.Test.ServiceSchedules.QueryTest;

public class GetServiceScheduleQueryHandlerTest
{
    private readonly GetServiceScheduleQueryHandler _queryHandler;
    private readonly Mock<IServiceScheduleRepository> _mockServiceScheduleRepository = new();
    private readonly Mock<IAppLogger<GetServiceScheduleQueryHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    public GetServiceScheduleQueryHandlerTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new ServiceScheduleMappingProfile());
            cfg.AddProfile(new ServiceTaskMappingProfile());
        });
        config.AssertConfigurationIsValid();

        _mapper = config.CreateMapper();
        _queryHandler = new GetServiceScheduleQueryHandler(_mockServiceScheduleRepository.Object, _mockLogger.Object, _mapper);
    }

    [Fact]
    public async Task Returns_Details_For_Valid_ID()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var serviceProgram = new ServiceProgram
        {
            ID = 2,
            CreatedAt = now,
            UpdatedAt = now,
            Name = "Test Program",
            IsActive = true,
            XrefServiceProgramVehicles = [],
            ServiceSchedules = []
        };
        var serviceSchedule = new ServiceSchedule
        {
            ID = 1,
            CreatedAt = now,
            UpdatedAt = now,
            ServiceProgramID = 2,
            Name = "Test Schedule",
            TimeIntervalValue = 10,
            TimeIntervalUnit = TimeUnitEnum.Days,
            TimeBufferValue = 2,
            TimeBufferUnit = TimeUnitEnum.Days,
            MileageInterval = 1000,
            MileageBuffer = 100,
            FirstServiceDate = DateTime.Today.AddDays(5),
            FirstServiceMileage = 500,
            IsActive = true,
            ServiceProgram = serviceProgram,
            XrefServiceScheduleServiceTasks =
            [
                new XrefServiceScheduleServiceTask
                {
                    ServiceScheduleID = 1,
                    ServiceTaskID = 1,
                    ServiceSchedule = null!, // Not needed for test
                    ServiceTask = new ServiceTask
                    {
                        ID = 1,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Name = "Task 1",
                        EstimatedLabourHours = 1.0,
                        EstimatedCost = 100,
                        Category = ServiceTaskCategoryEnum.PREVENTIVE,
                        IsActive = true,
                        XrefServiceScheduleServiceTasks = [],
                        MaintenanceHistories = [],
                        WorkOrderLineItems = []
                    }
                },
                new XrefServiceScheduleServiceTask
                {
                    ServiceScheduleID = 1,
                    ServiceTaskID = 2,
                    ServiceSchedule = null!, // Not needed for test
                    ServiceTask = new ServiceTask
                    {
                        ID = 2,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Name = "Task 2",
                        EstimatedLabourHours = 2.0,
                        EstimatedCost = 200,
                        Category = ServiceTaskCategoryEnum.CORRECTIVE,
                        IsActive = true,
                        XrefServiceScheduleServiceTasks = [],
                        MaintenanceHistories = [],
                        WorkOrderLineItems = []
                    }
                }
            ]
        };
        _mockServiceScheduleRepository.Setup(r => r.GetByIdWithServiceTasksAsync(1)).ReturnsAsync(serviceSchedule);
        var query = new GetServiceScheduleQuery(1);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(serviceSchedule.ID, result.ID);
        Assert.Equal(serviceSchedule.Name, result.Name);
        Assert.Equal(serviceSchedule.ServiceProgramID, result.ServiceProgramID);
        Assert.Equal(serviceSchedule.TimeIntervalValue, result.TimeIntervalValue);
        Assert.Equal(serviceSchedule.TimeIntervalUnit, result.TimeIntervalUnit);
        Assert.Equal(serviceSchedule.TimeBufferValue, result.TimeBufferValue);
        Assert.Equal(serviceSchedule.TimeBufferUnit, result.TimeBufferUnit);
        Assert.Equal(serviceSchedule.MileageInterval, result.MileageInterval);
        Assert.Equal(serviceSchedule.MileageBuffer, result.MileageBuffer);
        Assert.Equal(serviceSchedule.FirstServiceMileage, result.FirstServiceMileage);
        Assert.Equal(serviceSchedule.IsActive, result.IsActive);
        Assert.NotNull(result.ServiceTasks);
        Assert.Equal(2, result.ServiceTasks.Count);
        Assert.Contains(result.ServiceTasks, t => t.ID == 1 && t.Name == "Task 1");
        Assert.Contains(result.ServiceTasks, t => t.ID == 2 && t.Name == "Task 2");
    }

    [Fact]
    public async Task Throws_EntityNotFoundException_For_Invalid_ID()
    {
        // Arrange
        _mockServiceScheduleRepository.Setup(r => r.GetByIdWithServiceTasksAsync(99)).ReturnsAsync((ServiceSchedule)null!);
        var query = new GetServiceScheduleQuery(99);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _queryHandler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Maps_Navigation_Properties_Correctly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var navServiceProgram = new ServiceProgram
        {
            ID = 4,
            CreatedAt = now,
            UpdatedAt = now,
            Name = "Nav Program",
            IsActive = true,
            XrefServiceProgramVehicles = [],
            ServiceSchedules = []
        };
        var navSchedule = new ServiceSchedule
        {
            ID = 3,
            CreatedAt = now,
            UpdatedAt = now,
            ServiceProgramID = 4,
            Name = "Nav Test",
            TimeIntervalValue = 6,
            TimeIntervalUnit = TimeUnitEnum.Weeks,
            IsActive = true,
            ServiceProgram = navServiceProgram,
            XrefServiceScheduleServiceTasks =
            [
                new XrefServiceScheduleServiceTask
                {
                    ServiceScheduleID = 3,
                    ServiceTaskID = 3,
                    ServiceSchedule = null!,
                    ServiceTask = new ServiceTask
                    {
                        ID = 3,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Name = "Task 3",
                        EstimatedLabourHours = 1.5,
                        EstimatedCost = 150,
                        Category = ServiceTaskCategoryEnum.INSPECTION,
                        IsActive = true,
                        XrefServiceScheduleServiceTasks = [],
                        MaintenanceHistories = [],
                        WorkOrderLineItems = []
                    }
                },
                new XrefServiceScheduleServiceTask
                {
                    ServiceScheduleID = 3,
                    ServiceTaskID = 4,
                    ServiceSchedule = null!,
                    ServiceTask = new ServiceTask
                    {
                        ID = 4,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Name = "Task 4",
                        EstimatedLabourHours = 2.5,
                        EstimatedCost = 250,
                        Category = ServiceTaskCategoryEnum.WARRANTY,
                        IsActive = true,
                        XrefServiceScheduleServiceTasks = [],
                        MaintenanceHistories = [],
                        WorkOrderLineItems = []
                    }
                },
                new XrefServiceScheduleServiceTask
                {
                    ServiceScheduleID = 3,
                    ServiceTaskID = 5,
                    ServiceSchedule = null!,
                    ServiceTask = new ServiceTask
                    {
                        ID = 5,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Name = "Task 5",
                        EstimatedLabourHours = 3.5,
                        EstimatedCost = 350,
                        Category = ServiceTaskCategoryEnum.EMERGENCY,
                        IsActive = true,
                        XrefServiceScheduleServiceTasks = [],
                        MaintenanceHistories = [],
                        WorkOrderLineItems = []
                    }
                }
            ]
        };
        _mockServiceScheduleRepository.Setup(r => r.GetByIdWithServiceTasksAsync(3)).ReturnsAsync(navSchedule);
        var navQuery = new GetServiceScheduleQuery(3);

        // Act
        var result = await _queryHandler.Handle(navQuery, CancellationToken.None);

        // Assert
        // Only check direct properties
        Assert.Equal(navSchedule.ID, result.ID);
        Assert.Equal(navSchedule.Name, result.Name);
        Assert.Equal(navSchedule.ServiceProgramID, result.ServiceProgramID);
        Assert.Equal(navSchedule.IsActive, result.IsActive);
        Assert.NotNull(result.ServiceTasks);
        Assert.Equal(3, result.ServiceTasks.Count);
        Assert.Contains(result.ServiceTasks, t => t.ID == 3 && t.Name == "Task 3");
        Assert.Contains(result.ServiceTasks, t => t.ID == 4 && t.Name == "Task 4");
        Assert.Contains(result.ServiceTasks, t => t.ID == 5 && t.Name == "Task 5");
    }
}
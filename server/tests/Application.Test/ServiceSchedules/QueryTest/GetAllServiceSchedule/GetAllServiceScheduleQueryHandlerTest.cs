using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceSchedules.Query.GetAllServiceSchedule;
using Application.Features.ServiceTasks.Query.GetAllServiceTask;
using Application.MappingProfiles;
using Application.Models;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

using Xunit;

namespace Application.Test.ServiceSchedules.QueryTest.GetAllServiceSchedule;

public class GetAllServiceScheduleQueryHandlerTest
{
    private readonly GetAllServiceScheduleQueryHandler _queryHandler;
    private readonly Mock<IServiceScheduleRepository> _mockServiceScheduleRepository = new();
    private readonly Mock<IValidator<GetAllServiceScheduleQuery>> _mockValidator = new();
    private readonly Mock<IAppLogger<GetAllServiceScheduleQueryHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    public GetAllServiceScheduleQueryHandlerTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceScheduleMappingProfile>();
            cfg.AddProfile<ServiceTaskMappingProfile>();
        });
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();
        _queryHandler = new GetAllServiceScheduleQueryHandler(
            _mockServiceScheduleRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper
        );
    }

    private ServiceProgram CreateServiceProgram(int id = 1, string name = "Test Program")
    {
        return new ServiceProgram
        {
            ID = id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = name,
            IsActive = true,
            // Navigation Properties
            VehicleServicePrograms = [],
            ServiceSchedules = []
        };
    }

    private ServiceTask CreateServiceTask(int id, string name, ServiceTaskCategoryEnum category)
    {
        return new ServiceTask
        {
            ID = id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = name,
            Description = $"Description for {name}",
            EstimatedLabourHours = 1.0 * id,
            EstimatedCost = 100 * id,
            Category = category,
            IsActive = true,
            // Navigation Properties
            XrefServiceScheduleServiceTasks = [],
            MaintenanceHistories = [],
            WorkOrderLineItems = []
        };
    }

    private ServiceSchedule CreateServiceSchedule(int id = 1, int programId = 1, string name = "Test Schedule", List<XrefServiceScheduleServiceTask>? xrefs = null)
    {
        return new ServiceSchedule
        {
            ID = id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ServiceProgramID = programId,
            Name = name,
            TimeIntervalValue = 10,
            TimeIntervalUnit = TimeUnitEnum.Days,
            TimeBufferValue = 2,
            TimeBufferUnit = TimeUnitEnum.Days,
            MileageInterval = 1000,
            MileageBuffer = 100,
            FirstServiceTimeValue = 5,
            FirstServiceTimeUnit = TimeUnitEnum.Days,
            FirstServiceMileage = 500,
            IsActive = true,
            // Navigation Properties
            ServiceProgram = null!,
            XrefServiceScheduleServiceTasks = []
        };
    }

    private XrefServiceScheduleServiceTask CreateXref(int scheduleId, int taskId, ServiceTask? task = null)
    {
        return new XrefServiceScheduleServiceTask
        {
            ServiceScheduleID = scheduleId,
            ServiceTaskID = taskId,
            // Navigation Properties
            ServiceSchedule = null!,
            ServiceTask = null!
        };
    }

    private void SetupValidValidation(GetAllServiceScheduleQuery query)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(GetAllServiceScheduleQuery query, string propertyName = "Parameters.PageNumber", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator
            .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_With_ServiceTasks_On_Success()
    {
        // Arrange
        var program = CreateServiceProgram(1, "Test Program");
        var task1 = CreateServiceTask(1, "Oil Change", ServiceTaskCategoryEnum.PREVENTIVE);
        var task2 = CreateServiceTask(2, "Brake Inspection", ServiceTaskCategoryEnum.INSPECTION);
        var schedule = CreateServiceSchedule(10, program.ID, "5000 km / 6 month service");
        var xref1 = CreateXref(schedule.ID, task1.ID);
        xref1.ServiceTask = task1;
        var xref2 = CreateXref(schedule.ID, task2.ID);
        xref2.ServiceTask = task2;
        var xrefs = new List<XrefServiceScheduleServiceTask> { xref1, xref2 };
        schedule.XrefServiceScheduleServiceTasks = xrefs;
        var schedules = new List<ServiceSchedule> { schedule };
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 2,
            Search = "5000",
            SortBy = "name",
            SortDescending = false
        };
        var query = new GetAllServiceScheduleQuery(parameters);
        SetupValidValidation(query);
        _mockServiceScheduleRepository
            .Setup(r => r.GetAllServiceSchedulesPagedAsync(It.IsAny<PaginationParameters>()))
            .ReturnsAsync(new PagedResult<ServiceSchedule>
            {
                Items = schedules,
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 2
            });

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        var dto = result.Items[0];
        Assert.Equal(10, dto.ID);
        Assert.Equal("5000 km / 6 month service", dto.Name);
        Assert.Equal(2, dto.ServiceTasks.Count);
        Assert.Contains(dto.ServiceTasks, t => t.Name == "Oil Change");
        Assert.Contains(dto.ServiceTasks, t => t.Name == "Brake Inspection");
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_Schedules()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10, Search = "none" };
        var query = new GetAllServiceScheduleQuery(parameters);
        SetupValidValidation(query);
        _mockServiceScheduleRepository
            .Setup(r => r.GetAllServiceSchedulesPagedAsync(It.IsAny<PaginationParameters>()))
            .ReturnsAsync(new PagedResult<ServiceSchedule>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 10
            });

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 0, PageSize = 10 };
        var query = new GetAllServiceScheduleQuery(parameters);
        SetupInvalidValidation(query, "Parameters.PageNumber", "Page number must be greater than 0");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _queryHandler.Handle(query, CancellationToken.None));
        _mockServiceScheduleRepository.Verify(r => r.GetAllServiceSchedulesPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Return_Schedules_With_Multiple_Tasks()
    {
        // Arrange
        var program = CreateServiceProgram(1, "Test Program");
        var task1 = CreateServiceTask(1, "Task 1", ServiceTaskCategoryEnum.PREVENTIVE);
        var task2 = CreateServiceTask(2, "Task 2", ServiceTaskCategoryEnum.CORRECTIVE);
        var task3 = CreateServiceTask(3, "Task 3", ServiceTaskCategoryEnum.INSPECTION);
        var schedule = CreateServiceSchedule(1, program.ID, "Multi-Task Schedule");
        var xref1 = CreateXref(schedule.ID, task1.ID); xref1.ServiceTask = task1;
        var xref2 = CreateXref(schedule.ID, task2.ID); xref2.ServiceTask = task2;
        var xref3 = CreateXref(schedule.ID, task3.ID); xref3.ServiceTask = task3;
        var xrefs = new List<XrefServiceScheduleServiceTask> { xref1, xref2, xref3 };
        schedule.XrefServiceScheduleServiceTasks = xrefs;
        var schedules = new List<ServiceSchedule> { schedule };
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetAllServiceScheduleQuery(parameters);
        SetupValidValidation(query);
        _mockServiceScheduleRepository
            .Setup(r => r.GetAllServiceSchedulesPagedAsync(It.IsAny<PaginationParameters>()))
            .ReturnsAsync(new PagedResult<ServiceSchedule>
            {
                Items = schedules,
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            });

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        var dto = result.Items[0];
        Assert.NotNull(dto.ServiceTasks);
        Assert.Equal(3, dto.ServiceTasks.Count);
        Assert.Contains(dto.ServiceTasks, t => t.Name == "Task 1");
        Assert.Contains(dto.ServiceTasks, t => t.Name == "Task 2");
        Assert.Contains(dto.ServiceTasks, t => t.Name == "Task 3");
    }

    [Fact]
    public async Task Handler_Should_Handle_Different_Page_Sizes()
    {
        // Arrange
        var program = CreateServiceProgram(1, "Test Program");
        var schedules = new List<ServiceSchedule>();
        for (int i = 1; i <= 7; i++)
        {
            var schedule = CreateServiceSchedule(i, program.ID, $"Schedule {i}");
            schedules.Add(schedule);
        }
        var parameters = new PaginationParameters { PageNumber = 2, PageSize = 3 };
        var query = new GetAllServiceScheduleQuery(parameters);
        SetupValidValidation(query);
        _mockServiceScheduleRepository
            .Setup(r => r.GetAllServiceSchedulesPagedAsync(It.IsAny<PaginationParameters>()))
            .ReturnsAsync(new PagedResult<ServiceSchedule>
            {
                Items = schedules.Skip(3).Take(3).ToList(),
                TotalCount = 7,
                PageNumber = 2,
                PageSize = 3
            });

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(4, result.Items[0].ID);
    }

    [Fact]
    public async Task Handler_Should_Handle_Last_Page()
    {
        // Arrange
        var program = CreateServiceProgram(1, "Test Program");
        var schedules = new List<ServiceSchedule>();
        for (int i = 1; i <= 8; i++)
        {
            var schedule = CreateServiceSchedule(i, program.ID, $"Schedule {i}");
            schedules.Add(schedule);
        }
        var parameters = new PaginationParameters { PageNumber = 3, PageSize = 3 };
        var query = new GetAllServiceScheduleQuery(parameters);
        SetupValidValidation(query);
        _mockServiceScheduleRepository
            .Setup(r => r.GetAllServiceSchedulesPagedAsync(It.IsAny<PaginationParameters>()))
            .ReturnsAsync(new PagedResult<ServiceSchedule>
            {
                Items = schedules.Skip(6).Take(3).ToList(),
                TotalCount = 8,
                PageNumber = 3,
                PageSize = 3
            });

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(8, result.TotalCount);
        Assert.Equal(3, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(2, result.Items.Count); // Only 2 items on last page
        Assert.Equal(7, result.Items[0].ID);
        Assert.Equal(8, result.Items[1].ID);
    }
}
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.ServiceReminders.Query;
using Application.Features.ServiceReminders.Query.GetAllServiceReminders;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.ServiceReminders.QueryTest.GetAllServiceReminders;

public class GetAllServiceRemindersQueryHandlerTest
{
    private readonly Mock<IServiceReminderRepository> _mockServiceReminderRepository;
    private readonly Mock<IValidator<GetAllServiceRemindersQuery>> _mockValidator;
    private readonly Mock<IAppLogger<GetAllServiceRemindersQueryHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetAllServiceRemindersQueryHandler _handler;

    public GetAllServiceRemindersQueryHandlerTest()
    {
        _mockServiceReminderRepository = new Mock<IServiceReminderRepository>();
        _mockValidator = new Mock<IValidator<GetAllServiceRemindersQuery>>();
        _mockLogger = new Mock<IAppLogger<GetAllServiceRemindersQueryHandler>>();
        _mockMapper = new Mock<IMapper>();

        _handler = new GetAllServiceRemindersQueryHandler(
            _mockServiceReminderRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_Result_When_No_Reminders()
    {
        // Arrange
        var query = new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Setup valid validator result
        _mockValidator.Setup(v => v.Validate(query))
            .Returns(new FluentValidation.Results.ValidationResult());

        // Setup empty result from repository
        var emptyResult = new PagedResult<ServiceReminderDTO>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _mockServiceReminderRepository.Setup(r => r.GetAllCalculatedServiceRemindersPagedAsync(query.Parameters))
            .ReturnsAsync(emptyResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);

        // Verify repository interaction
        _mockServiceReminderRepository.Verify(r => r.GetAllCalculatedServiceRemindersPagedAsync(query.Parameters), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Multiple_Reminders()
    {
        // Arrange
        var query = new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Setup valid validator result
        _mockValidator.Setup(v => v.Validate(query))
            .Returns(new FluentValidation.Results.ValidationResult());

        // Setup result with multiple reminders
        var expectedReminders = CreateMultipleTestReminderDTOs();
        var expectedResult = new PagedResult<ServiceReminderDTO>
        {
            Items = expectedReminders,
            TotalCount = expectedReminders.Count,
            PageNumber = 1,
            PageSize = 10
        };

        _mockServiceReminderRepository.Setup(r => r.GetAllCalculatedServiceRemindersPagedAsync(query.Parameters))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.Equal(expectedReminders.Count, result.TotalCount);

        // Verify repository interaction
        _mockServiceReminderRepository.Verify(r => r.GetAllCalculatedServiceRemindersPagedAsync(query.Parameters), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Filtered_Results_For_Search()
    {
        // Arrange
        var searchTerm = "Vehicle1";
        var query = new GetAllServiceRemindersQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = searchTerm
        });

        // Setup valid validator result
        _mockValidator.Setup(v => v.Validate(query))
            .Returns(new FluentValidation.Results.ValidationResult());

        // Setup filtered search result
        var filteredReminders = CreateTestReminderDTOsForSearch("Vehicle1");
        var searchResult = new PagedResult<ServiceReminderDTO>
        {
            Items = filteredReminders,
            TotalCount = filteredReminders.Count,
            PageNumber = 1,
            PageSize = 10
        };

        _mockServiceReminderRepository.Setup(r => r.GetAllCalculatedServiceRemindersPagedAsync(query.Parameters))
            .ReturnsAsync(searchResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result.Items, r => r.VehicleName.Contains("Vehicle1"));
        Assert.Equal(filteredReminders.Count, result.TotalCount);

        // Verify repository interaction
        _mockServiceReminderRepository.Verify(r => r.GetAllCalculatedServiceRemindersPagedAsync(query.Parameters), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_For_Invalid_Query()
    {
        // Arrange
        var query = new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = -1, PageSize = 10 });

        // Setup invalid validator result
        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("PageNumber", "Page number must be greater than 0"));

        _mockValidator.Setup(v => v.Validate(query))
            .Returns(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<Application.Exceptions.BadRequestException>(() => _handler.Handle(query, CancellationToken.None));

        // Verify repository was not called
        _mockServiceReminderRepository.Verify(r => r.GetAllCalculatedServiceRemindersPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }

    #region Helper Methods

    private static List<ServiceReminderDTO> CreateMultipleTestReminderDTOs()
    {
        return
        [
            new ServiceReminderDTO
            {
                VehicleID = 1,
                VehicleName = "Test Vehicle 1",
                ServiceProgramID = 1,
                ServiceProgramName = "Test Program",
                ServiceScheduleID = 1,
                ServiceScheduleName = "Test Schedule 1",
                ServiceTasks = [
                    new ServiceTaskInfoDTO
                    {
                        ServiceTaskID = 1,
                        ServiceTaskName = "Oil Change",
                        ServiceTaskCategory = ServiceTaskCategoryEnum.PREVENTIVE,
                        EstimatedLabourHours = 1.5,
                        EstimatedCost = 75.00m,
                        Description = "Change engine oil and filter",
                        IsRequired = true
                    }
                ],
                TotalEstimatedLabourHours = 1.5,
                TotalEstimatedCost = 75.00m,
                TaskCount = 1,
                DueDate = DateTime.UtcNow.AddDays(-30),
                Status = ServiceReminderStatusEnum.OVERDUE,
                PriorityLevel = PriorityLevelEnum.HIGH,
                CurrentMileage = 25000,
                OccurrenceNumber = 1,
                IsTimeBasedReminder = true,
                IsMileageBasedReminder = false
            },
            new ServiceReminderDTO
            {
                VehicleID = 1,
                VehicleName = "Test Vehicle 1",
                ServiceProgramID = 1,
                ServiceProgramName = "Test Program",
                ServiceScheduleID = 1,
                ServiceScheduleName = "Test Schedule 1",
                ServiceTasks = [
                    new ServiceTaskInfoDTO
                    {
                        ServiceTaskID = 1,
                        ServiceTaskName = "Oil Change",
                        ServiceTaskCategory = ServiceTaskCategoryEnum.PREVENTIVE,
                        EstimatedLabourHours = 1.5,
                        EstimatedCost = 75.00m,
                        Description = "Change engine oil and filter",
                        IsRequired = true
                    }
                ],
                TotalEstimatedLabourHours = 1.5,
                TotalEstimatedCost = 75.00m,
                TaskCount = 1,
                DueMileage = 20000,
                Status = ServiceReminderStatusEnum.OVERDUE,
                PriorityLevel = PriorityLevelEnum.HIGH,
                CurrentMileage = 25000,
                OccurrenceNumber = 1,
                IsTimeBasedReminder = false,
                IsMileageBasedReminder = true
            }
        ];
    }

    private static List<ServiceReminderDTO> CreateTestReminderDTOsForSearch(string vehicleName)
    {
        return
        [
            new ServiceReminderDTO
            {
                VehicleID = 1,
                VehicleName = vehicleName,
                ServiceProgramID = 1,
                ServiceProgramName = "Test Program",
                ServiceScheduleID = 1,
                ServiceScheduleName = "Test Schedule",
                ServiceTasks = [
                    new ServiceTaskInfoDTO
                    {
                        ServiceTaskID = 1,
                        ServiceTaskName = "Oil Change",
                        ServiceTaskCategory = ServiceTaskCategoryEnum.PREVENTIVE,
                        EstimatedLabourHours = 1.5,
                        EstimatedCost = 75.00m,
                        Description = "Change engine oil and filter",
                        IsRequired = true
                    }
                ],
                TotalEstimatedLabourHours = 1.5,
                TotalEstimatedCost = 75.00m,
                TaskCount = 1,
                Status = ServiceReminderStatusEnum.UPCOMING,
                PriorityLevel = PriorityLevelEnum.LOW,
                CurrentMileage = 15000,
                OccurrenceNumber = 1,
                IsTimeBasedReminder = true,
                IsMileageBasedReminder = false
            }
        ];
    }

    #endregion
}
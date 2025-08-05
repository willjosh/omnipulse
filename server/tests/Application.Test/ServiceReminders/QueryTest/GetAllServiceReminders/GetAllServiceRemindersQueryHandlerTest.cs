using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceReminders.Query;
using Application.Features.ServiceReminders.Query.GetAllServiceReminders;
using Application.Models.PaginationModels;

using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.ServiceReminders.QueryTest.GetAllServiceReminders;

public class GetAllServiceRemindersQueryHandlerTest
{
    private readonly Mock<IServiceReminderRepository> _mockServiceReminderRepository;
    private readonly Mock<IValidator<GetAllServiceRemindersQuery>> _mockValidator;
    private readonly Mock<IAppLogger<GetAllServiceRemindersQueryHandler>> _mockLogger;
    private readonly GetAllServiceRemindersQueryHandler _handler;

    public GetAllServiceRemindersQueryHandlerTest()
    {
        _mockServiceReminderRepository = new Mock<IServiceReminderRepository>();
        _mockValidator = new Mock<IValidator<GetAllServiceRemindersQuery>>();
        _mockLogger = new Mock<IAppLogger<GetAllServiceRemindersQueryHandler>>();

        _handler = new GetAllServiceRemindersQueryHandler(
            _mockServiceReminderRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_Result_When_No_Reminders()
    {
        // Arrange
        var query = new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Setup valid validator result
        _mockValidator.Setup(v => v.Validate(query))
            .Returns(new ValidationResult());

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

        _mockServiceReminderRepository.Verify(r => r.GetAllCalculatedServiceRemindersPagedAsync(query.Parameters), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Reminders_When_Available()
    {
        // Arrange
        var query = new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Setup valid validator result
        _mockValidator.Setup(v => v.Validate(query))
            .Returns(new ValidationResult());

        var dtoReminders = new List<ServiceReminderDTO>
        {
            CreateTestServiceReminderDTO(1, "Vehicle 1", ServiceReminderStatusEnum.OVERDUE),
            CreateTestServiceReminderDTO(2, "Vehicle 2", ServiceReminderStatusEnum.DUE_SOON)
        };

        var result = new PagedResult<ServiceReminderDTO>
        {
            Items = dtoReminders,
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 10
        };

        _mockServiceReminderRepository.Setup(r => r.GetAllCalculatedServiceRemindersPagedAsync(query.Parameters))
            .ReturnsAsync(result);

        // Act
        var actualResult = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(actualResult);
        Assert.Equal(2, actualResult.Items.Count);
        Assert.Equal(2, actualResult.TotalCount);
        Assert.Equal(1, actualResult.PageNumber);
        Assert.Equal(10, actualResult.PageSize);

        _mockServiceReminderRepository.Verify(r => r.GetAllCalculatedServiceRemindersPagedAsync(query.Parameters), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var query = new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = 0, PageSize = 10 });

        // Setup invalid validator result
        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure("PageNumber", "Page number must be greater than 0"));

        _mockValidator.Setup(v => v.Validate(query))
            .Returns(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(query, CancellationToken.None));

        // Verify repository was never called
        _mockServiceReminderRepository.Verify(r => r.GetAllCalculatedServiceRemindersPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }


    private static ServiceReminderDTO CreateTestServiceReminderDTO(int vehicleId, string vehicleName, ServiceReminderStatusEnum status)
    {
        return new ServiceReminderDTO
        {
            VehicleID = vehicleId,
            VehicleName = vehicleName,
            ServiceScheduleID = 1,
            ServiceScheduleName = "Test Schedule",
            ServiceTasks = [],
            TotalEstimatedLabourHours = 2.0,
            TotalEstimatedCost = 100.00m,
            TaskCount = 1,
            Status = status,
            PriorityLevel = PriorityLevelEnum.MEDIUM,
            CurrentMileage = 10000.0,
            OccurrenceNumber = 1,
            IsTimeBasedReminder = true,
            IsMileageBasedReminder = false
        };
    }
}
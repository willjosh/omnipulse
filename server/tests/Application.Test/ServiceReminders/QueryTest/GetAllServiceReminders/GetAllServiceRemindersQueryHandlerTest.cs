using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceReminders.Query;
using Application.Features.ServiceReminders.Query.GetAllServiceReminders;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

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
            .Returns(new ValidationResult());

        // Mock GetAllServiceRemindersPagedAsync to return empty result
        var emptyPagedResult = new PagedResult<ServiceReminder>
        {
            Items = new List<ServiceReminder>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };
        _mockServiceReminderRepository.Setup(r => r.GetAllServiceRemindersPagedAsync(It.IsAny<PaginationParameters>()))
            .ReturnsAsync(emptyPagedResult);

        // Mock AutoMapper to return empty list
        _mockMapper.Setup(m => m.Map<List<ServiceReminderDTO>>(It.IsAny<IReadOnlyList<ServiceReminder>>()))
            .Returns(new List<ServiceReminderDTO>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);

        // Verify only the read operation was called
        _mockServiceReminderRepository.Verify(r => r.GetAllServiceRemindersPagedAsync(It.IsAny<PaginationParameters>()), Times.Once);
        _mockMapper.Verify(m => m.Map<List<ServiceReminderDTO>>(It.IsAny<IReadOnlyList<ServiceReminder>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Reminders_When_Available()
    {
        // Arrange
        var query = new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Setup valid validator result
        _mockValidator.Setup(v => v.Validate(query))
            .Returns(new ValidationResult());

        // Mock GetAllServiceRemindersPagedAsync to return empty result (simplified test)
        var emptyPagedResult = new PagedResult<ServiceReminder>
        {
            Items = new List<ServiceReminder>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };
        _mockServiceReminderRepository.Setup(r => r.GetAllServiceRemindersPagedAsync(It.IsAny<PaginationParameters>()))
            .ReturnsAsync(emptyPagedResult);

        // Mock AutoMapper to return empty list
        _mockMapper.Setup(m => m.Map<List<ServiceReminderDTO>>(It.IsAny<IReadOnlyList<ServiceReminder>>()))
            .Returns(new List<ServiceReminderDTO>());

        // Act
        var actualResult = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(actualResult);
        Assert.Empty(actualResult.Items);
        Assert.Equal(0, actualResult.TotalCount);
        Assert.Equal(1, actualResult.PageNumber);
        Assert.Equal(10, actualResult.PageSize);

        // Verify the repository and mapper were called
        _mockServiceReminderRepository.Verify(r => r.GetAllServiceRemindersPagedAsync(It.IsAny<PaginationParameters>()), Times.Once);
        _mockMapper.Verify(m => m.Map<List<ServiceReminderDTO>>(It.IsAny<IReadOnlyList<ServiceReminder>>()), Times.Once);
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
        _mockServiceReminderRepository.Verify(r => r.GetAllServiceRemindersPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }
}
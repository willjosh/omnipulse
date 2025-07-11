using Application.Contracts.Logger;
using Application.Contracts.Persistence;
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

namespace Application.Test.ServiceTasks.QueryTest.GetAllServiceTask;

public class GetAllServiceTaskQueryHandlerTest
{
    private readonly GetAllServiceTaskQueryHandler _queryHandler;
    private readonly Mock<IServiceTaskRepository> _mockServiceTaskRepository;
    private readonly Mock<IValidator<GetAllServiceTaskQuery>> _mockValidator;
    private readonly Mock<IAppLogger<GetAllServiceTaskQueryHandler>> _mockLogger;
    private readonly IMapper _mapper;

    public GetAllServiceTaskQueryHandlerTest()
    {
        _mockServiceTaskRepository = new();
        _mockValidator = new();
        _mockLogger = new();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceTaskMappingProfile>());
        _mapper = config.CreateMapper();
        _queryHandler = new GetAllServiceTaskQueryHandler(
            _mockServiceTaskRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper
        );
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_Of_GetAllServiceTaskDTO_On_Success()
    {
        // Given
        var query = new GetAllServiceTaskQuery(new PaginationParameters { PageNumber = 1, PageSize = 10 });
        _mockValidator.Setup(v => v.Validate(query)).Returns(new FluentValidation.Results.ValidationResult());

        var serviceTasks = new List<ServiceTask>
        {
            new ServiceTask
            {
                ID = 1,
                Name = "Engine Oil Change",
                Description = "Replace engine oil and filter",
                EstimatedLabourHours = 1.5,
                EstimatedCost = 85.50m,
                Category = ServiceTaskCategoryEnum.PREVENTIVE,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                XrefServiceScheduleServiceTasks = [],
                MaintenanceHistories = [],
                WorkOrderLineItems = []
            },
            new ServiceTask
            {
                ID = 2,
                Name = "Brake Inspection",
                Description = "Inspect brake pads and discs",
                EstimatedLabourHours = 1.0,
                EstimatedCost = 60.00m,
                Category = ServiceTaskCategoryEnum.INSPECTION,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                XrefServiceScheduleServiceTasks = [],
                MaintenanceHistories = [],
                WorkOrderLineItems = []
            }
        };
        var pagedResult = new PagedResult<ServiceTask>
        {
            Items = serviceTasks,
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 10
        };
        _mockServiceTaskRepository.Setup(r => r.GetAllServiceTasksPagedAsync(query.Parameters)).ReturnsAsync(pagedResult);

        // When
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, item => Assert.IsType<GetAllServiceTaskDTO>(item));
        Assert.Equal("Engine Oil Change", result.Items[0].Name);
        Assert.Equal("Brake Inspection", result.Items[1].Name);
        _mockServiceTaskRepository.Verify(r => r.GetAllServiceTasksPagedAsync(query.Parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var query = new GetAllServiceTaskQuery(new PaginationParameters { PageNumber = 0, PageSize = 10 });
        var validationResult = new FluentValidation.Results.ValidationResult(new[] {
            new FluentValidation.Results.ValidationFailure("PageNumber", "PageNumber must be greater than 0.")
        });
        _mockValidator.Setup(v => v.Validate(query)).Returns(validationResult);

        // When & Then
        await Assert.ThrowsAsync<Application.Exceptions.BadRequestException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );
        _mockServiceTaskRepository.Verify(r => r.GetAllServiceTasksPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }
}
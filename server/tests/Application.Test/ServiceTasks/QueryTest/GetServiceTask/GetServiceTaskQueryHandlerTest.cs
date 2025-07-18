using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceTasks.Query;
using Application.Features.ServiceTasks.Query.GetServiceTask;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using Moq;

using Xunit;

namespace Application.Test.ServiceTasks.QueryTest.GetServiceTask;

public class GetServiceTaskQueryHandlerTest
{
    private readonly GetServiceTaskQueryHandler _getServiceTaskQueryHandler;
    private readonly Mock<IServiceTaskRepository> _mockServiceTaskRepository;
    private readonly Mock<IAppLogger<GetServiceTaskQueryHandler>> _mockLogger;

    public GetServiceTaskQueryHandlerTest()
    {
        _mockServiceTaskRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceTaskMappingProfile>());
        var mapper = config.CreateMapper();

        _getServiceTaskQueryHandler = new GetServiceTaskQueryHandler(
            _mockServiceTaskRepository.Object,
            _mockLogger.Object,
            mapper
        );
    }

    [Fact]
    public async Task Handler_Should_Return_GetServiceTaskDTO_On_Success()
    {
        // Given
        var query = new GetServiceTaskQuery(1);

        var expectedServiceTask = new ServiceTask
        {
            ID = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "Engine Oil Change",
            Description = "Replace engine oil and filter",
            EstimatedLabourHours = 1.5,
            EstimatedCost = 85.50m,
            Category = ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive = true,
            XrefServiceScheduleServiceTasks = [],
            MaintenanceHistories = [],
            WorkOrderLineItems = []
        };

        _mockServiceTaskRepository.Setup(r => r.GetByIdAsync(query.ServiceTaskID))
            .ReturnsAsync(expectedServiceTask);

        // When
        var result = await _getServiceTaskQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.IsType<ServiceTaskDTO>(result);
        Assert.Equal(expectedServiceTask.ID, result.ID);
        Assert.Equal(expectedServiceTask.Name, result.Name);
        Assert.Equal(expectedServiceTask.Description, result.Description);
        Assert.Equal(expectedServiceTask.EstimatedLabourHours, result.EstimatedLabourHours);
        Assert.Equal(expectedServiceTask.EstimatedCost, result.EstimatedCost);
        Assert.Equal(expectedServiceTask.Category, result.Category);
        Assert.Equal(expectedServiceTask.IsActive, result.IsActive);

        _mockServiceTaskRepository.Verify(r => r.GetByIdAsync(query.ServiceTaskID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_NonExistent_ServiceTaskID()
    {
        // Given
        var nonExistentId = 999;
        var query = new GetServiceTaskQuery(nonExistentId);

        _mockServiceTaskRepository.Setup(r => r.GetByIdAsync(nonExistentId))
            .ReturnsAsync((ServiceTask?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _getServiceTaskQueryHandler.Handle(query, CancellationToken.None)
        );

        _mockServiceTaskRepository.Verify(r => r.GetByIdAsync(nonExistentId), Times.Once);
    }
}
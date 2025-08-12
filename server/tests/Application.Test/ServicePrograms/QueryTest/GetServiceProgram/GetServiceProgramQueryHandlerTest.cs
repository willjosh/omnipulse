using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServicePrograms.Query;
using Application.Features.ServicePrograms.Query.GetServiceProgram;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using Moq;

namespace Application.Test.ServicePrograms.QueryTest.GetServiceProgram;

public class GetServiceProgramQueryHandlerTest
{
    private readonly GetServiceProgramQueryHandler _queryHandler;
    private readonly Mock<IServiceProgramRepository> _mockServiceProgramRepository = new();
    private readonly Mock<IServiceScheduleRepository> _mockServiceScheduleRepository = new();
    private readonly Mock<IXrefServiceScheduleServiceTaskRepository> _mockXrefServiceScheduleServiceTaskRepository = new();
    private readonly Mock<IServiceTaskRepository> _mockServiceTaskRepository = new();
    private readonly Mock<IXrefServiceProgramVehicleRepository> _mockXrefServiceProgramVehicleRepository = new();

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public GetServiceProgramQueryHandlerTest()
    {
        var mockLogger = new Mock<IAppLogger<GetServiceProgramQueryHandler>>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceProgramMappingProfile>();
            cfg.AddProfile<ServiceScheduleMappingProfile>();
            cfg.AddProfile<ServiceTaskMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _queryHandler = new GetServiceProgramQueryHandler(
            _mockServiceProgramRepository.Object,
            _mockServiceScheduleRepository.Object,
            _mockXrefServiceScheduleServiceTaskRepository.Object,
            _mockServiceTaskRepository.Object,
            _mockXrefServiceProgramVehicleRepository.Object,
            mockLogger.Object,
            mapper
        );
    }

    [Fact]
    public async Task Handler_Should_Return_ServiceProgramDTO_On_Success()
    {
        // Arrange
        var query = new GetServiceProgramQuery(1);

        var expectedServiceProgram = new ServiceProgram
        {
            ID = 1,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Name = "Fleet Maintenance Program",
            Description = "Comprehensive maintenance program for fleet vehicles",
            IsActive = true,
            ServiceSchedules = [
                new ServiceSchedule
                {
                    ID = 1,
                    CreatedAt = FixedDate,
                    UpdatedAt = FixedDate,
                    ServiceProgramID = 1,
                    Name = "Oil Change Schedule",
                    TimeIntervalValue = 3,
                    TimeIntervalUnit = TimeUnitEnum.Weeks,
                    MileageInterval = null, // XOR: time-based only
                    XrefServiceScheduleServiceTasks = [],
                    ServiceProgram = null!
                }
            ],
            XrefServiceProgramVehicles = [
                new XrefServiceProgramVehicle
                {
                    ServiceProgramID = 1,
                    VehicleID = 1,
                    AddedAt = FixedDate,
                    ServiceProgram = null!,
                    Vehicle = null!,
                    VehicleMileageAtAssignment = 10000,
                    // User = null! // TODO XrefServiceProgramVehicle User
                }
            ]
        };

        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(query.ServiceProgramID))
            .ReturnsAsync(expectedServiceProgram);
        _mockServiceScheduleRepository.Setup(r => r.GetAllByServiceProgramIDAsync(1))
            .ReturnsAsync(expectedServiceProgram.ServiceSchedules.ToList());
        _mockXrefServiceScheduleServiceTaskRepository.Setup(r => r.GetByServiceScheduleIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([]);
        _mockServiceTaskRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([]);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetByServiceProgramIDAsync(1))
            .ReturnsAsync(expectedServiceProgram.XrefServiceProgramVehicles.ToList());

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ServiceProgramDTO>(result);
        Assert.Equal(expectedServiceProgram.ID, result.ID);
        Assert.Equal(expectedServiceProgram.Name, result.Name);
        Assert.Equal(expectedServiceProgram.Description, result.Description);
        Assert.Equal(expectedServiceProgram.IsActive, result.IsActive);
        Assert.NotNull(result.ServiceSchedules);
        Assert.Single(result.ServiceSchedules);
        Assert.NotNull(result.AssignedVehicleIDs);
        Assert.Single(result.AssignedVehicleIDs);

        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(query.ServiceProgramID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_NonExistent_ServiceProgramID()
    {
        // Arrange
        var nonExistentId = 999;
        var query = new GetServiceProgramQuery(nonExistentId);

        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(nonExistentId))
            .ReturnsAsync((ServiceProgram?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(nonExistentId), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Collections_When_No_Schedules_Or_Vehicles()
    {
        // Arrange
        var query = new GetServiceProgramQuery(1);

        var expectedServiceProgram = new ServiceProgram
        {
            ID = 1,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Name = "Empty Program",
            Description = "Program with no schedules or vehicles",
            IsActive = true,
            ServiceSchedules = [],
            XrefServiceProgramVehicles = []
        };

        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(query.ServiceProgramID))
            .ReturnsAsync(expectedServiceProgram);
        _mockServiceScheduleRepository.Setup(r => r.GetAllByServiceProgramIDAsync(1))
            .ReturnsAsync([]);
        _mockXrefServiceScheduleServiceTaskRepository.Setup(r => r.GetByServiceScheduleIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([]);
        _mockServiceTaskRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([]);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetByServiceProgramIDAsync(1))
            .ReturnsAsync([]);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedServiceProgram.ID, result.ID);
        Assert.NotNull(result.ServiceSchedules);
        Assert.Empty(result.ServiceSchedules);
        Assert.NotNull(result.AssignedVehicleIDs);
        Assert.Empty(result.AssignedVehicleIDs);

        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(query.ServiceProgramID), Times.Once);
    }
}
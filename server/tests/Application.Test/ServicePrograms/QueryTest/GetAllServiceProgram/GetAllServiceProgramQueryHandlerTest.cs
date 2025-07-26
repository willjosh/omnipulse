using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServicePrograms.Query.GetAllServiceProgram;
using Application.MappingProfiles;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using Moq;

using Xunit;

namespace Application.Test.ServicePrograms.QueryTest.GetAllServiceProgram;

public class GetAllServiceProgramQueryHandlerTest
{
    private readonly GetAllServiceProgramQueryHandler _queryHandler;
    private readonly Mock<IServiceProgramRepository> _mockServiceProgramRepository = new();
    private readonly Mock<IValidator<GetAllServiceProgramQuery>> _mockValidator = new();

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public GetAllServiceProgramQueryHandlerTest()
    {
        var mockLogger = new Mock<IAppLogger<GetAllServiceProgramQueryHandler>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceProgramMappingProfile>());
        var mapper = config.CreateMapper();

        _queryHandler = new GetAllServiceProgramQueryHandler(
            _mockServiceProgramRepository.Object,
            _mockValidator.Object,
            mockLogger.Object,
            mapper
        );
    }

    private void SetupValidValidation(GetAllServiceProgramQuery command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.Validate(command))
            .Returns(validResult);
    }

    private void SetupInvalidValidation(GetAllServiceProgramQuery command, string propertyName = nameof(GetAllServiceProgramQuery.Parameters), string errorMessage = "Invalid Validation")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.Validate(command))
            .Returns(invalidResult);
    }

    private static List<ServiceProgram> CreateServicePrograms()
    {
        return
        [
            new ServiceProgram
            {
                ID = 1,
                CreatedAt = FixedDate,
                UpdatedAt = FixedDate,
                Name = "Fleet Maintenance Program",
                Description = "Comprehensive maintenance program for fleet vehicles",
                IsActive = true,
                ServiceSchedules =
                [
                    new ServiceSchedule { ID = 1, CreatedAt = FixedDate, UpdatedAt = FixedDate, ServiceProgramID = 1, Name = "Oil Change", IsActive = true, ServiceProgram = null!, XrefServiceScheduleServiceTasks = [] },
                    new ServiceSchedule { ID = 2, CreatedAt = FixedDate, UpdatedAt = FixedDate, ServiceProgramID = 1, Name = "Tire Rotation", IsActive = true, ServiceProgram = null!, XrefServiceScheduleServiceTasks = [] }
                ],
                XrefServiceProgramVehicles =
                [
                    new XrefServiceProgramVehicle { ServiceProgramID = 1, VehicleID = 1, AddedAt = FixedDate, ServiceProgram = null!, Vehicle = null! }, // TODO XrefServiceProgramVehicle User
                    new XrefServiceProgramVehicle { ServiceProgramID = 1, VehicleID = 2, AddedAt = FixedDate, ServiceProgram = null!, Vehicle = null! }, // TODO XrefServiceProgramVehicle User
                    new XrefServiceProgramVehicle { ServiceProgramID = 1, VehicleID = 3, AddedAt = FixedDate, ServiceProgram = null!, Vehicle = null! } // TODO XrefServiceProgramVehicle User
                ]
            },
            new ServiceProgram
            {
                ID = 2,
                CreatedAt = FixedDate.AddDays(-1),
                UpdatedAt = FixedDate.AddDays(-1),
                Name = "Emergency Response Program",
                Description = "Emergency vehicle maintenance program",
                IsActive = true,
                ServiceSchedules =
                [
                    new ServiceSchedule { ID = 3, CreatedAt = FixedDate, UpdatedAt = FixedDate, ServiceProgramID = 2, Name = "Emergency Check", IsActive = true, ServiceProgram = null!, XrefServiceScheduleServiceTasks = [] }
                ],
                XrefServiceProgramVehicles =
                [
                    new XrefServiceProgramVehicle { ServiceProgramID = 2, VehicleID = 4, AddedAt = FixedDate, ServiceProgram = null!, Vehicle = null! } // TODO XrefServiceProgramVehicle User
                ]
            },
            new ServiceProgram
            {
                ID = 3,
                CreatedAt = FixedDate.AddDays(-2),
                UpdatedAt = FixedDate.AddDays(-2),
                Name = "Inactive Program",
                Description = "This program is inactive",
                IsActive = false,
                ServiceSchedules = [],
                XrefServiceProgramVehicles = []
            }
        ];
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_On_Success()
    {
        // Arrange
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var query = new GetAllServiceProgramQuery(parameters);
        SetupValidValidation(query);

        var sampleServicePrograms = CreateServicePrograms();
        var pagedResult = new PagedResult<ServiceProgram>
        {
            Items = sampleServicePrograms,
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };

        _mockServiceProgramRepository.Setup(r => r.GetAllServiceProgramsPagedAsync(parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResult<GetAllServiceProgramDTO>>(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.Items.Count);

        // Verify first service program mapping
        var firstServiceProgram = result.Items[0];
        Assert.Equal(1, firstServiceProgram.ID);
        Assert.Equal("Fleet Maintenance Program", firstServiceProgram.Name);
        Assert.Equal("Comprehensive maintenance program for fleet vehicles", firstServiceProgram.Description);
        Assert.True(firstServiceProgram.IsActive);
        Assert.Equal(2, firstServiceProgram.ServiceScheduleCount);
        Assert.Equal(3, firstServiceProgram.AssignedVehicleCount);
        Assert.Equal(FixedDate, firstServiceProgram.CreatedAt);
        Assert.Equal(FixedDate, firstServiceProgram.UpdatedAt);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetAllServiceProgramsPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_PagedResult_When_No_ServicePrograms_Found()
    {
        // Arrange
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var query = new GetAllServiceProgramQuery(parameters);
        SetupValidValidation(query);

        var pagedResult = new PagedResult<ServiceProgram>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _mockServiceProgramRepository.Setup(r => r.GetAllServiceProgramsPagedAsync(parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Empty(result.Items);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetAllServiceProgramsPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Pagination_Correctly()
    {
        // Arrange
        var parameters = new PaginationParameters
        {
            PageNumber = 2,
            PageSize = 5
        };

        var query = new GetAllServiceProgramQuery(parameters);
        SetupValidValidation(query);

        var sampleServicePrograms = CreateServicePrograms().Take(2).ToList(); // Simulate page 2 with 2 items
        var pagedResult = new PagedResult<ServiceProgram>
        {
            Items = sampleServicePrograms,
            TotalCount = 12,
            PageNumber = 2,
            PageSize = 5
        };

        _mockServiceProgramRepository.Setup(r => r.GetAllServiceProgramsPagedAsync(parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(2, result.Items.Count);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetAllServiceProgramsPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Search_Parameters()
    {
        // Arrange
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "Fleet"
        };

        var query = new GetAllServiceProgramQuery(parameters);
        SetupValidValidation(query);

        var filteredServicePrograms = CreateServicePrograms().Where(sp => sp.Name.Contains("Fleet")).ToList();
        var pagedResult = new PagedResult<ServiceProgram>
        {
            Items = filteredServicePrograms,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _mockServiceProgramRepository.Setup(r => r.GetAllServiceProgramsPagedAsync(parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Contains("Fleet", result.Items[0].Name);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetAllServiceProgramsPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Sorting_Parameters()
    {
        // Arrange
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            SortBy = "name",
            SortDescending = true
        };

        var query = new GetAllServiceProgramQuery(parameters);
        SetupValidValidation(query);

        var sortedServicePrograms = CreateServicePrograms().OrderByDescending(sp => sp.Name).ToList();
        var pagedResult = new PagedResult<ServiceProgram>
        {
            Items = sortedServicePrograms,
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };

        _mockServiceProgramRepository.Setup(r => r.GetAllServiceProgramsPagedAsync(parameters))
                                   .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
        // Verify sorting (should be in descending order by name)
        Assert.Equal("Inactive Program", result.Items[0].Name);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetAllServiceProgramsPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Map_ServiceProgram_Counts_Correctly()
    {
        // Arrange
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var query = new GetAllServiceProgramQuery(parameters);
        SetupValidValidation(query);

        var servicePrograms = CreateServicePrograms();
        var firstServiceProgram = servicePrograms.First(); // This has 2 schedules and 3 vehicles

        var pagedResult = new PagedResult<ServiceProgram>
        {
            Items = [firstServiceProgram],
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _mockServiceProgramRepository.Setup(r => r.GetAllServiceProgramsPagedAsync(parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.Items);
        var dto = result.Items[0];
        Assert.Equal(2, dto.ServiceScheduleCount); // Fleet Maintenance Program has 2 schedules
        Assert.Equal(3, dto.AssignedVehicleCount); // Fleet Maintenance Program has 3 vehicles
        Assert.Equal("Fleet Maintenance Program", dto.Name);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetAllServiceProgramsPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Arrange
        var parameters = new PaginationParameters
        {
            PageNumber = 0,
            PageSize = 10
        };

        var query = new GetAllServiceProgramQuery(parameters);
        SetupInvalidValidation(query, "Parameters.PageNumber", "Page number must be greater than 0");

        // Act & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetAllServiceProgramsPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }
}
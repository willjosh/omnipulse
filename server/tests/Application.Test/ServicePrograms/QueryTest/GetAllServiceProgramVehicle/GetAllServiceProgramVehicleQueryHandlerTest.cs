using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServicePrograms.Query.GetAllServiceProgramVehicle;
using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using Moq;

using Xunit;

namespace Application.Test.ServicePrograms.QueryTest.GetAllServiceProgramVehicle;

public class GetAllServiceProgramVehicleQueryHandlerTest
{
    private readonly GetAllServiceProgramVehicleQueryHandler _queryHandler;
    private readonly Mock<IServiceProgramRepository> _mockServiceProgramRepository = new();
    private readonly Mock<IXrefServiceProgramVehicleRepository> _mockXrefServiceProgramVehicleRepository = new();
    private readonly Mock<IValidator<GetAllServiceProgramVehicleQuery>> _mockValidator = new();

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public GetAllServiceProgramVehicleQueryHandlerTest()
    {
        var mockLogger = new Mock<IAppLogger<GetAllServiceProgramVehicleQueryHandler>>();

        _queryHandler = new GetAllServiceProgramVehicleQueryHandler(
            _mockServiceProgramRepository.Object,
            _mockXrefServiceProgramVehicleRepository.Object,
            _mockValidator.Object,
            mockLogger.Object);
    }

    private void SetupValidValidation(GetAllServiceProgramVehicleQuery query)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(GetAllServiceProgramVehicleQuery query, string propertyName = nameof(GetAllServiceProgramVehicleQuery.ServiceProgramID), string errorMessage = "Invalid Validation")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    private static GetAllServiceProgramVehicleQuery CreateValidQuery(
        int serviceProgramId = 1,
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null,
        string? sortBy = null,
        bool sortDescending = false)
    {
        var parameters = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Search = search,
            SortBy = sortBy,
            SortDescending = sortDescending
        };
        return new GetAllServiceProgramVehicleQuery(serviceProgramId, parameters);
    }

    private static ServiceProgram CreateServiceProgram(
        int id = 1,
        string name = "Test Service Program Name",
        string description = "Test Service Program Description",
        bool isActive = true)
    {
        return new ServiceProgram
        {
            ID = id,
            Name = name,
            Description = description,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            IsActive = isActive,
            ServiceSchedules = [],
            XrefServiceProgramVehicles = []
        };
    }

    private static XrefServiceProgramVehicleDTO CreateXrefServiceProgramVehicleDTO(
        int serviceProgramId = 1,
        int vehicleId = 1,
        string vehicleName = "Test Vehicle Name",
        DateTime? addedAt = null)
    {
        return new XrefServiceProgramVehicleDTO
        {
            ServiceProgramID = serviceProgramId,
            VehicleID = vehicleId,
            VehicleName = vehicleName,
            AddedAt = addedAt ?? FixedDate
        };
    }

    private static PagedResult<XrefServiceProgramVehicleDTO> CreatePagedResult(
        List<XrefServiceProgramVehicleDTO> items,
        int totalCount = 0,
        int pageNumber = 1,
        int pageSize = 10)
    {
        return new PagedResult<XrefServiceProgramVehicleDTO>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    [Fact]
    public async Task Handler_Should_Return_Paginated_Vehicles_On_Success()
    {
        // Arrange
        var serviceProgramId = 1;
        var query = CreateValidQuery(serviceProgramId);
        var serviceProgram = CreateServiceProgram(serviceProgramId);
        var xrefDTO = CreateXrefServiceProgramVehicleDTO(serviceProgramId);
        var pagedResult = CreatePagedResult([xrefDTO], 1);

        SetupValidValidation(query);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
            .ReturnsAsync(serviceProgram);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetServiceProgramVehiclesWithMetadataPagedAsync(serviceProgramId, query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal(serviceProgramId, result.Items[0].ServiceProgramID);
        Assert.Equal(xrefDTO.VehicleID, result.Items[0].VehicleID);
        Assert.Equal(xrefDTO.VehicleName, result.Items[0].VehicleName);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var query = CreateValidQuery(0);
        SetupInvalidValidation(query, nameof(GetAllServiceProgramVehicleQuery.ServiceProgramID), "Invalid ID");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_ServiceProgram_Not_Found()
    {
        // Arrange
        var serviceProgramId = 999;
        var query = CreateValidQuery(serviceProgramId);

        SetupValidValidation(query);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
            .ReturnsAsync((ServiceProgram?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(serviceProgramId), Times.Once);
        _mockXrefServiceProgramVehicleRepository.Verify(r => r.GetServiceProgramVehiclesWithMetadataPagedAsync(It.IsAny<int>(), It.IsAny<PaginationParameters>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_Vehicles_Found()
    {
        // Arrange
        var serviceProgramId = 1;
        var query = CreateValidQuery(serviceProgramId);
        var serviceProgram = CreateServiceProgram(serviceProgramId, "Empty Service Program Name");
        var emptyPagedResult = CreatePagedResult([], 0);

        SetupValidValidation(query);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
            .ReturnsAsync(serviceProgram);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetServiceProgramVehiclesWithMetadataPagedAsync(serviceProgramId, query.Parameters))
            .ReturnsAsync(emptyPagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task Handler_Should_Propagate_Repository_Exception()
    {
        // Arrange
        var serviceProgramId = 1;
        var query = CreateValidQuery(serviceProgramId);
        var serviceProgram = CreateServiceProgram(serviceProgramId, "Test Program");

        SetupValidValidation(query);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
            .ReturnsAsync(serviceProgram);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetServiceProgramVehiclesWithMetadataPagedAsync(serviceProgramId, It.IsAny<PaginationParameters>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(serviceProgramId), Times.Once);
        _mockXrefServiceProgramVehicleRepository.Verify(r => r.GetServiceProgramVehiclesWithMetadataPagedAsync(serviceProgramId, It.IsAny<PaginationParameters>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Pass_Search_And_Sort_Parameters_To_Repository()
    {
        // Arrange
        var serviceProgramId = 1;
        var query = CreateValidQuery(serviceProgramId, 2, 5, "Ford", "vehiclename", true);
        var serviceProgram = CreateServiceProgram(serviceProgramId, "Test Program");
        var pagedResult = CreatePagedResult([], 0, 2, 5);

        SetupValidValidation(query);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
            .ReturnsAsync(serviceProgram);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetServiceProgramVehiclesWithMetadataPagedAsync(serviceProgramId, query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);

        // Verify that the repository was called with the exact parameters
        _mockXrefServiceProgramVehicleRepository.Verify(
            r => r.GetServiceProgramVehiclesWithMetadataPagedAsync(serviceProgramId, query.Parameters),
            Times.Once
        );
    }

    [Fact]
    public async Task Handler_Should_Return_Multiple_Vehicles_When_Available()
    {
        // Arrange
        var serviceProgramId = 1;
        var query = CreateValidQuery(serviceProgramId);
        var serviceProgram = CreateServiceProgram(serviceProgramId, "Test Program");
        var multipleVehicles = new List<XrefServiceProgramVehicleDTO>
        {
            CreateXrefServiceProgramVehicleDTO(serviceProgramId, 1, "Ford F-150"),
            CreateXrefServiceProgramVehicleDTO(serviceProgramId, 2, "Chevrolet Silverado"),
            CreateXrefServiceProgramVehicleDTO(serviceProgramId, 3, "Toyota Tacoma")
        };
        var pagedResult = CreatePagedResult(multipleVehicles, 3);

        SetupValidValidation(query);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
            .ReturnsAsync(serviceProgram);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetServiceProgramVehiclesWithMetadataPagedAsync(serviceProgramId, query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);

        // Verify each vehicle
        Assert.Equal("Ford F-150", result.Items[0].VehicleName);
        Assert.Equal(1, result.Items[0].VehicleID);
        Assert.Equal("Chevrolet Silverado", result.Items[1].VehicleName);
        Assert.Equal(2, result.Items[1].VehicleID);
        Assert.Equal("Toyota Tacoma", result.Items[2].VehicleName);
        Assert.Equal(3, result.Items[2].VehicleID);
    }
}
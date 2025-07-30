using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServicePrograms.Query.GetAllServiceProgramVehicle;
using Application.Models.PaginationModels;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using Moq;

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

    private static Vehicle CreateVehicle(
        int id = 1,
        string name = "Test Vehicle Name",
        string make = "Test Make",
        string model = "Test Model")
    {
        return new Vehicle
        {
            ID = id,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Name = name,
            Make = make,
            Model = model,
            Year = 2020,
            VIN = "TEST123456789",
            LicensePlate = "TEST123",
            LicensePlateExpirationDate = FixedDate.AddYears(1),
            VehicleType = Domain.Entities.Enums.VehicleTypeEnum.TRUCK,
            VehicleGroupID = 1,
            Trim = "Test Trim",
            Mileage = 1000.0,
            EngineHours = 100.0,
            FuelCapacity = 50.0,
            FuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate = FixedDate,
            PurchasePrice = 50000.0m,
            Status = Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location = "Test Location",
            VehicleGroup = new VehicleGroup
            {
                ID = 1,
                Name = "Test Group",
                CreatedAt = FixedDate,
                UpdatedAt = FixedDate,
                IsActive = true
            },
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };
    }

    private static XrefServiceProgramVehicle CreateXrefServiceProgramVehicle(
        int serviceProgramId = 1,
        int vehicleId = 1,
        Vehicle? vehicle = null)
    {
        return new XrefServiceProgramVehicle
        {
            ServiceProgramID = serviceProgramId,
            VehicleID = vehicleId,
            AddedAt = FixedDate,
            ServiceProgram = CreateServiceProgram(serviceProgramId),
            Vehicle = vehicle ?? CreateVehicle(vehicleId),
            // User = new User // TODO XrefServiceProgramVehicle User
            // {
            //     Id = "test-user",
            //     Email = "test@example.com",
            //     FirstName = "Test",
            //     LastName = "User",
            //     HireDate = FixedDate,
            //     IsActive = true,
            //     CreatedAt = FixedDate,
            //     UpdatedAt = FixedDate,
            //     MaintenanceHistories = [],
            //     IssueAttachments = [],
            //     VehicleAssignments = [],
            //     VehicleDocuments = [],
            //     VehicleInspections = [],
            //     Vehicles = []
            // }
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

    private static PagedResult<XrefServiceProgramVehicle> CreatePagedXrefResult(
        List<XrefServiceProgramVehicle> items,
        int totalCount = 0,
        int pageNumber = 1,
        int pageSize = 10)
    {
        return new PagedResult<XrefServiceProgramVehicle>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
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
        var vehicle = CreateVehicle(1, "Test Vehicle Name");
        var xref = CreateXrefServiceProgramVehicle(serviceProgramId, 1, vehicle);
        var pagedXrefResult = CreatePagedXrefResult([xref], 1);

        SetupValidValidation(query);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
            .ReturnsAsync(serviceProgram);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetAllByServiceProgramIDPagedAsync(serviceProgramId, query.Parameters))
            .ReturnsAsync(pagedXrefResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal(serviceProgramId, result.Items[0].ServiceProgramID);
        Assert.Equal(vehicle.ID, result.Items[0].VehicleID);
        Assert.Equal(vehicle.Name, result.Items[0].VehicleName);
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
        _mockXrefServiceProgramVehicleRepository.Verify(r => r.GetAllByServiceProgramIDPagedAsync(It.IsAny<int>(), It.IsAny<PaginationParameters>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_Vehicles_Found()
    {
        // Arrange
        var serviceProgramId = 1;
        var query = CreateValidQuery(serviceProgramId);
        var serviceProgram = CreateServiceProgram(serviceProgramId, "Empty Service Program Name");
        var emptyPagedResult = CreatePagedXrefResult([], 0);

        SetupValidValidation(query);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
            .ReturnsAsync(serviceProgram);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetAllByServiceProgramIDPagedAsync(serviceProgramId, query.Parameters))
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
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetAllByServiceProgramIDPagedAsync(serviceProgramId, It.IsAny<PaginationParameters>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(serviceProgramId), Times.Once);
        _mockXrefServiceProgramVehicleRepository.Verify(r => r.GetAllByServiceProgramIDPagedAsync(serviceProgramId, It.IsAny<PaginationParameters>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Pass_Search_And_Sort_Parameters_To_Repository()
    {
        // Arrange
        var serviceProgramId = 1;
        var query = CreateValidQuery(serviceProgramId, 2, 5, "Ford", "vehiclename", true);
        var serviceProgram = CreateServiceProgram(serviceProgramId, "Test Program");
        var pagedResult = CreatePagedXrefResult([], 0, 2, 5);

        SetupValidValidation(query);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
            .ReturnsAsync(serviceProgram);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetAllByServiceProgramIDPagedAsync(serviceProgramId, query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);

        // Verify that the repository was called with the exact parameters
        _mockXrefServiceProgramVehicleRepository.Verify(
            r => r.GetAllByServiceProgramIDPagedAsync(serviceProgramId, query.Parameters),
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
        var vehicle1 = CreateVehicle(1, "Ford F-150");
        var vehicle2 = CreateVehicle(2, "Chevrolet Silverado");
        var vehicle3 = CreateVehicle(3, "Toyota Tacoma");

        var multipleXrefs = new List<XrefServiceProgramVehicle>
        {
            CreateXrefServiceProgramVehicle(serviceProgramId, 1, vehicle1),
            CreateXrefServiceProgramVehicle(serviceProgramId, 2, vehicle2),
            CreateXrefServiceProgramVehicle(serviceProgramId, 3, vehicle3)
        };
        var pagedResult = CreatePagedXrefResult(multipleXrefs, 3);

        SetupValidValidation(query);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
            .ReturnsAsync(serviceProgram);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetAllByServiceProgramIDPagedAsync(serviceProgramId, query.Parameters))
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

    [Fact]
    public async Task Handler_Should_Throw_ArgumentNullException_When_Request_Is_Null()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _queryHandler.Handle(null!, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handler_Should_Throw_ArgumentNullException_When_Request_Parameters_Is_Null()
    {
        // Arrange
        var query = new GetAllServiceProgramVehicleQuery(1, null!);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );
    }

    // TODO
    // [Fact]
    // public async Task Handler_Should_Throw_BadRequestException_When_ServiceProgram_Is_Inactive()
    // {
    //     // Arrange
    //     var serviceProgramId = 1;
    //     var query = CreateValidQuery(serviceProgramId);
    //     var inactiveServiceProgram = CreateServiceProgram(serviceProgramId, "Inactive Program", isActive: false);

    //     SetupValidValidation(query);
    //     _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
    //         .ReturnsAsync(inactiveServiceProgram);

    //     // Act & Assert
    //     var exception = await Assert.ThrowsAsync<BadRequestException>(
    //         () => _queryHandler.Handle(query, CancellationToken.None)
    //     );

    //     Assert.Contains($"ServiceProgram with ID {serviceProgramId} is not active.", exception.Message);

    //     _mockValidator.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
    //     _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(serviceProgramId), Times.Once);
    //     _mockXrefServiceProgramVehicleRepository.Verify(r => r.GetAllByServiceProgramIDPagedAsync(It.IsAny<int>(), It.IsAny<PaginationParameters>()), Times.Never);
    // }
}
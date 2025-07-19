using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServicePrograms.Query.GetAllServiceProgramVehicle;
using Application.MappingProfiles;
using Application.Models;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

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
        var config = new MapperConfiguration(cfg => cfg.AddProfile<XrefServiceProgramVehicleMappingProfile>());
        var mapper = config.CreateMapper();

        _queryHandler = new GetAllServiceProgramVehicleQueryHandler(
            _mockServiceProgramRepository.Object,
            _mockXrefServiceProgramVehicleRepository.Object,
            _mockValidator.Object,
            mockLogger.Object,
            mapper
        );
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
        int pageSize = 10)
    {
        var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
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
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Name = name,
            Description = description,
            IsActive = isActive,
            ServiceSchedules = [],
            XrefServiceProgramVehicles = []
        };
    }

    private static Vehicle CreateVehicle(
        int id = 1,
        string name = "Test Vehicle Name",
        string make = "Ford",
        string model = "F-150",
        int year = 2022,
        string vin = "1FTFW1ET5DFC12345",
        string licensePlate = "ABC123",
        VehicleTypeEnum vehicleType = VehicleTypeEnum.TRUCK,
        int vehicleGroupId = 1)
    {
        return new Vehicle
        {
            ID = id,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Name = name,
            Make = make,
            Model = model,
            Year = year,
            VIN = vin,
            LicensePlate = licensePlate,
            LicensePlateExpirationDate = FixedDate.AddYears(1),
            VehicleType = vehicleType,
            VehicleGroupID = vehicleGroupId,
            Trim = "XLT",
            Mileage = 15000,
            EngineHours = 500,
            FuelCapacity = 80,
            FuelType = FuelTypeEnum.PETROL,
            PurchaseDate = new DateTime(2022, 1, 1),
            PurchasePrice = 35000,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "Warehouse A",
            User = null!,
            VehicleGroup = null!,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };
    }

    private static XrefServiceProgramVehicle CreateVehicleAssignment(
        int serviceProgramId = 1,
        int vehicleId = 1,
        DateTime? addedAt = null,
        ServiceProgram? serviceProgram = null,
        Vehicle? vehicle = null)
    {
        return new XrefServiceProgramVehicle
        {
            ServiceProgramID = serviceProgramId,
            VehicleID = vehicleId,
            AddedAt = addedAt ?? DateTime.UtcNow,
            ServiceProgram = serviceProgram!,
            Vehicle = vehicle!,
            User = null!
        };
    }

    private static PagedResult<XrefServiceProgramVehicle> CreatePagedResult(
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

    [Fact]
    public async Task Handle_Should_Return_Paginated_Vehicles_On_Success()
    {
        // Arrange
        var serviceProgramId = 1;
        var query = CreateValidQuery(serviceProgramId);
        var serviceProgram = CreateServiceProgram(serviceProgramId);
        var vehicle = CreateVehicle();
        var vehicleAssignment = CreateVehicleAssignment(serviceProgramId, vehicle.ID, serviceProgram: serviceProgram, vehicle: vehicle);
        var pagedResult = CreatePagedResult([vehicleAssignment], 1);

        SetupValidValidation(query);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
            .ReturnsAsync(serviceProgram);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetAllByServiceProgramIDPagedAsync(serviceProgramId, query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);

        // Verify all repository calls
        _mockValidator.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(serviceProgramId), Times.Once);
        _mockXrefServiceProgramVehicleRepository.Verify(r => r.GetAllByServiceProgramIDPagedAsync(serviceProgramId, query.Parameters), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var query = CreateValidQuery(0);

        SetupInvalidValidation(query, nameof(GetAllServiceProgramVehicleQuery.ServiceProgramID), "ServiceProgramID must be greater than 0.");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockXrefServiceProgramVehicleRepository.Verify(r => r.GetAllByServiceProgramIDPagedAsync(It.IsAny<int>(), It.IsAny<PaginationParameters>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_ServiceProgram_Not_Found()
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
    public async Task Handle_Should_Handle_Null_Vehicle_Gracefully()
    {
        // Arrange
        var serviceProgramId = 1;
        var query = CreateValidQuery(serviceProgramId);
        var serviceProgram = CreateServiceProgram(serviceProgramId);
        var vehicleAssignment = CreateVehicleAssignment(serviceProgramId, 1, serviceProgram: serviceProgram, vehicle: null!);
        var pagedResult = CreatePagedResult([vehicleAssignment], 1);

        SetupValidValidation(query);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(serviceProgramId))
            .ReturnsAsync(serviceProgram);
        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetAllByServiceProgramIDPagedAsync(serviceProgramId, query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_Result_When_No_Vehicles_Found()
    {
        // Arrange
        var serviceProgramId = 1;
        var query = CreateValidQuery(serviceProgramId);
        var serviceProgram = CreateServiceProgram(serviceProgramId, "Empty Program");
        var emptyPagedResult = CreatePagedResult([], 0);

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
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }
}
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Vehicles.Query.GetAllVehicle;
using Application.MappingProfiles;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.Vehicles.QueryTest.GetAllVehicle;

public class GetAllVehicleQueryHandlerTest
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly GetAllVehicleQueryHandler _getAllVehicleQueryHandler;
    private readonly Mock<IAppLogger<GetAllVehicleQueryHandler>> _mockLogger;
    private readonly Mock<IValidator<GetAllVehicleQuery>> _mockValidator;

    public GetAllVehicleQueryHandlerTest()
    {
        _mockVehicleRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<VehicleMappingProfile>());
        var mapper = config.CreateMapper();

        _getAllVehicleQueryHandler = new GetAllVehicleQueryHandler(
            _mockVehicleRepository.Object,
            mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    // Helper method to set up valid validation result
    private void SetupValidValidation(GetAllVehicleQuery query)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(validResult);
    }

    // Helper method to set up validation failure
    private void SetupInvalidValidation(GetAllVehicleQuery query, string propertyName = "Parameters.PageNumber", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_On_Success()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 5,
            Search = "Toyota",
            SortBy = "Name",
            SortDescending = false
        };

        var query = new GetAllVehicleQuery(parameters);
        SetupValidValidation(query);

        // Create Vehicle entities (what the repository returns)
        var vehicleGroup = new VehicleGroup
        {
            ID = 2,
            Name = "Group 1",
            Description = "Test Group",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var user = new Domain.Entities.User
        {
            Id = "GUID123",
            FirstName = "John",
            LastName = "Doe",
            HireDate = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Vehicles = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            Inspections = [],
            MaintenanceHistories = [],
            IssueAttachments = []
        };

        var expectedVehicleEntities = new List<Vehicle>
        {
            new() {
                ID = 1,
                CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Name = "Toyota Corolla",
                Make = "Toyota",
                Model = "Corolla",
                Year = 2023,
                VIN = "1234567890ABCDEFG",
                LicensePlate = "ABC123",
                LicensePlateExpirationDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                VehicleType = VehicleTypeEnum.CAR,
                VehicleGroupID = 2,
                Trim = "LE",
                Mileage = 50000,
                EngineHours = 1000,
                FuelCapacity = 50.0,
                FuelType = FuelTypeEnum.PETROL,
                PurchaseDate = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                PurchasePrice = 25000.00m,
                Status = VehicleStatusEnum.ACTIVE,
                Location = "Sydney",
                AssignedTechnicianID = "GUID123",
                VehicleGroup = vehicleGroup,
                User = user,
                VehicleImages = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                XrefServiceProgramVehicles = [],
                ServiceReminders = [],
                Issues = [],
                Inspections = []
            },
            new() {
                ID = 2,
                CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Name = "Toyota Camry",
                Make = "Toyota",
                Model = "Camry",
                Year = 2023,
                VIN = "1234567890ABCDEFZ",
                LicensePlate = "ABC122",
                LicensePlateExpirationDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                VehicleType = VehicleTypeEnum.CAR,
                VehicleGroupID = 2,
                Trim = "LE",
                Mileage = 30000,
                EngineHours = 800,
                FuelCapacity = 60.0,
                FuelType = FuelTypeEnum.PETROL,
                PurchaseDate = new DateTime(2021, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                PurchasePrice = 30000.00m,
                Status = VehicleStatusEnum.ACTIVE,
                Location = "Melbourne",
                AssignedTechnicianID = "GUID123",
                VehicleGroup = vehicleGroup,
                User = user,
                VehicleImages = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                XrefServiceProgramVehicles = [],
                ServiceReminders = [],
                Issues = [],
                Inspections = []
            }
        };

        var pagedVehicleEntities = new PagedResult<Vehicle>
        {
            Items = expectedVehicleEntities,
            TotalCount = 25,
            PageNumber = 1,
            PageSize = 5
        };

        _mockVehicleRepository.Setup(r => r.GetAllVehiclesPagedAsync(parameters))
                              .ReturnsAsync(pagedVehicleEntities);

        // When
        var result = await _getAllVehicleQueryHandler.Handle(query, CancellationToken.None);

        // Then  
        Assert.NotNull(result);
        Assert.IsType<PagedResult<GetAllVehicleDTO>>(result);

        Assert.Equal(25, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(5, result.TotalPages); // 25 / 5 = 5 pages
        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);

        // Check items
        Assert.Equal(2, result.Items.Count);

        var firstVehicle = result.Items[0];
        Assert.Equal(1, firstVehicle.ID);
        Assert.Equal("Toyota Corolla", firstVehicle.Name);
        Assert.Equal("Toyota", firstVehicle.Make);
        Assert.Equal("Corolla", firstVehicle.Model);
        Assert.Equal(2023, firstVehicle.Year);
        Assert.Equal("1234567890ABCDEFG", firstVehicle.VIN);
        Assert.Equal("ABC123", firstVehicle.LicensePlate);
        Assert.Equal(VehicleTypeEnum.CAR, firstVehicle.VehicleType);
        Assert.Equal(2, firstVehicle.VehicleGroupID);
        Assert.Equal("Group 1", firstVehicle.VehicleGroupName); // Mapped from navigation property
        Assert.Equal("John Doe", firstVehicle.AssignedTechnicianName); // Mapped from navigation property
        Assert.Equal("GUID123", firstVehicle.AssignedTechnicianID);
        Assert.Equal("LE", firstVehicle.Trim);
        Assert.Equal(50000, firstVehicle.Mileage);
        Assert.Equal(1000, firstVehicle.EngineHours);
        Assert.Equal(50.0, firstVehicle.FuelCapacity);
        Assert.Equal(FuelTypeEnum.PETROL, firstVehicle.FuelType);
        Assert.Equal(25000.00m, firstVehicle.PurchasePrice);
        Assert.Equal(VehicleStatusEnum.ACTIVE, firstVehicle.Status);
        Assert.Equal("Sydney", firstVehicle.Location);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetAllVehiclesPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_Vehicles()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "NonExistentBrand"
        };

        var query = new GetAllVehicleQuery(parameters);
        SetupValidValidation(query);

        var emptyPagedResult = new PagedResult<Vehicle>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _mockVehicleRepository.Setup(r => r.GetAllVehiclesPagedAsync(parameters))
                              .ReturnsAsync(emptyPagedResult);

        // When
        var result = await _getAllVehicleQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetAllVehiclesPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Different_Page_Sizes()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 2,
            PageSize = 3
        };

        var query = new GetAllVehicleQuery(parameters);
        SetupValidValidation(query);

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = [], // Empty for simplicity
            TotalCount = 10,
            PageNumber = 2,
            PageSize = 3
        };

        _mockVehicleRepository.Setup(r => r.GetAllVehiclesPagedAsync(parameters))
                              .ReturnsAsync(pagedResult);

        // When
        var result = await _getAllVehicleQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(4, result.TotalPages); // 10 / 3 = 4 pages (rounded up)
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetAllVehiclesPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Last_Page()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 3,
            PageSize = 5
        };

        var query = new GetAllVehicleQuery(parameters);
        SetupValidValidation(query);

        var pagedResult = new PagedResult<Vehicle>
        {
            Items = [],
            TotalCount = 12,
            PageNumber = 3,
            PageSize = 5
        };

        _mockVehicleRepository.Setup(r => r.GetAllVehiclesPagedAsync(parameters))
                              .ReturnsAsync(pagedResult);

        // When
        var result = await _getAllVehicleQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(3, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetAllVehiclesPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 0,
            PageSize = 10
        };

        var query = new GetAllVehicleQuery(parameters);
        SetupInvalidValidation(query, "Parameters.PageNumber", "Page number must be greater than 0");

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _getAllVehicleQueryHandler.Handle(query, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetAllVehiclesPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }
}
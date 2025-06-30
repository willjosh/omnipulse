using System;
using System.Threading.Tasks;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Vehicles.Query.GetVehicleDetails;
using Application.MappingProfiles;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Enums;
using Moq;

namespace Application.Test.Vehicles.QueryTest.GetVehicleDetails;

public class GetVehicleDetailsQueryHandlerTest
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly GetVehicleDetailsQueryHandler _getVehicleDetailsQueryHandler;
    private readonly Mock<IAppLogger<GetVehicleDetailsQueryHandler>> _mockLogger;


    public GetVehicleDetailsQueryHandlerTest()
    {
        _mockVehicleRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VehicleMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _getVehicleDetailsQueryHandler = new GetVehicleDetailsQueryHandler(
            _mockVehicleRepository.Object,
            mapper,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task Handler_Should_Return_GetVehicleDetailsDTO_On_Success()
    {
        // Given
        var query = new GetVehicleDetailsQuery(1);

        // Create related entities first
        var expectedVehicleGroup = new VehicleGroup
        {
            ID = 2,
            Name = "Kuala Lumpur Group",
            Description = "Group for vehicles in Kuala Lumpur",
            IsActive = true,
            CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };

        var expectedUser = new User
        {
            Id = "GUID123",
            FirstName = "John",
            LastName = "Doe",
            HireDate = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Vehicles = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleInspections = [],
            MaintenanceHistories = [],
            IssueAttachments = []
        };

        var expectedVehicle = new Vehicle
        {
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
            VehicleType = Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID = 2,
            Trim = "LE",
            Mileage = 50000,
            EngineHours = 1000,
            FuelCapacity = 50.0,
            FuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePrice = 25000.00m,
            Status = Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location = "Sydney",
            AssignedTechnicianID = "GUID123",
            VehicleGroup = expectedVehicleGroup,
            User = expectedUser,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        // Setup the mock to return the vehicle with navigation properties loaded
        _mockVehicleRepository.Setup(r => r.GetVehicleWithDetailsAsync(query.VehicleID))
            .ReturnsAsync(expectedVehicle);

        // When
        var result = await _getVehicleDetailsQueryHandler.Handle(query, CancellationToken.None);

        // Then 
        Assert.NotNull(result);
        Assert.IsType<GetVehicleDetailsDTO>(result);
        Assert.Equal("Toyota Corolla", result.Name);
        Assert.Equal(1, result.ID);
        Assert.Equal("Kuala Lumpur Group", result.VehicleGroupName);
        Assert.Equal("John Doe", result.AssignedTechnicianName);
        Assert.Equal("GUID123", result.AssignedTechnicianID);
        Assert.Equal("Toyota", result.Make);
        Assert.Equal("Corolla", result.Model);
        Assert.Equal(2023, result.Year);
        Assert.Equal("1234567890ABCDEFG", result.VIN);
        Assert.Equal("ABC123", result.LicensePlate);
        Assert.Equal(VehicleTypeEnum.CAR, result.VehicleType);
        Assert.Equal("LE", result.Trim);
        Assert.Equal(50000, result.Mileage);
        Assert.Equal(1000, result.EngineHours);
        Assert.Equal(50.0, result.FuelCapacity);
        Assert.Equal(FuelTypeEnum.PETROL, result.FuelType);
        Assert.Equal(25000.00m, result.PurchasePrice);
        Assert.Equal(VehicleStatusEnum.ACTIVE, result.Status);
        Assert.Equal("Sydney", result.Location);

        _mockVehicleRepository.Verify(r => r.GetVehicleWithDetailsAsync(query.VehicleID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_GetVehicleDetailsDTO_With_No_AssignedTechnicianID()
    {
        // Given
        var query = new GetVehicleDetailsQuery(1);

        // Create related entities first
        var expectedVehicleGroup = new VehicleGroup
        {
            ID = 2,
            Name = "Kuala Lumpur Group",
            Description = "Group for vehicles in Kuala Lumpur",
            IsActive = true,
            CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };

        var expectedVehicle = new Vehicle
        {
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
            AssignedTechnicianID = null,
            VehicleGroup = expectedVehicleGroup,
            User = null,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        // Setup the mock to return the vehicle with navigation properties loaded
        _mockVehicleRepository.Setup(r => r.GetVehicleWithDetailsAsync(query.VehicleID))
            .ReturnsAsync(expectedVehicle);

        // When
        var result = await _getVehicleDetailsQueryHandler.Handle(query, CancellationToken.None);

        // Then 
        Assert.NotNull(result);
        Assert.IsType<GetVehicleDetailsDTO>(result);
        Assert.Equal("Toyota Corolla", result.Name);
        Assert.Equal(1, result.ID);
        Assert.Equal("Kuala Lumpur Group", result.VehicleGroupName);

        Assert.Equal("Not Assigned", result.AssignedTechnicianName);
        Assert.Equal(string.Empty, result.AssignedTechnicianID);

        Assert.Equal("Toyota", result.Make);
        Assert.Equal("Corolla", result.Model);
        Assert.Equal(2023, result.Year);
        Assert.Equal("1234567890ABCDEFG", result.VIN);
        Assert.Equal("ABC123", result.LicensePlate);
        Assert.Equal(VehicleTypeEnum.CAR, result.VehicleType);
        Assert.Equal("LE", result.Trim);
        Assert.Equal(50000, result.Mileage);
        Assert.Equal(1000, result.EngineHours);
        Assert.Equal(50.0, result.FuelCapacity);
        Assert.Equal(FuelTypeEnum.PETROL, result.FuelType);
        Assert.Equal(25000.00m, result.PurchasePrice);
        Assert.Equal(VehicleStatusEnum.ACTIVE, result.Status);
        Assert.Equal("Sydney", result.Location);

        _mockVehicleRepository.Verify(r => r.GetVehicleWithDetailsAsync(query.VehicleID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_NonExistent_VehicleID()
    {
        // Given
        var nonExistentVehicleId = 999;
        var query = new GetVehicleDetailsQuery(nonExistentVehicleId);

        _mockVehicleRepository.Setup(r => r.GetVehicleWithDetailsAsync(nonExistentVehicleId))
            .ReturnsAsync((Vehicle?)null);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _getVehicleDetailsQueryHandler.Handle(query, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetVehicleWithDetailsAsync(nonExistentVehicleId), Times.Once);
    }

}

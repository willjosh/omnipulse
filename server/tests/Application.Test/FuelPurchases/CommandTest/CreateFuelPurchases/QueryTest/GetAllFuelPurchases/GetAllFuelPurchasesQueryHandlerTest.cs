using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.MappingProfiles;
using Application.Models;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.FuelPurchases.QueryTest;

using System;

using Application.Features.FuelPurchases.Query.GetAllFuelPurchases;

public class GetAllFuelPurchasesQueryHandlerTest
{

    private readonly Mock<IFuelPurchaseRepository> _mockFuelPurchasesRepository;
    private readonly GetAllFuelPurchasesQueryHandler _getAllFuelPurchasesQueryHandler;
    private readonly Mock<IAppLogger<GetAllFuelPurchasesQueryHandler>> _mockLogger;
    private readonly IMapper _mapper;

    private readonly Mock<IValidator<GetAllFuelPurchasesQuery>> _mockValidator;
    public GetAllFuelPurchasesQueryHandlerTest()
    {
        _mockFuelPurchasesRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<FuelPurchaseMappingProfile>());

        _mapper = config.CreateMapper();

        _getAllFuelPurchasesQueryHandler = new GetAllFuelPurchasesQueryHandler(
            _mockFuelPurchasesRepository.Object,
            _mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    // Helper method to set up valid validation result
    private void SetupValidValidation(GetAllFuelPurchasesQuery query)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(validResult);
    }

    // Helper method to set up validation failure
    private void SetupInvalidValidation(GetAllFuelPurchasesQuery query, string propertyName = "Parameters.PageNumber", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(invalidResult);
    }

    [Fact]
    public async Task GetAllFuelPurchasesQueryould_Return_GetAllFuelPurchasesDTO_On_Success()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 5,
            Search = "",
            SortBy = "receiptnumber",
            SortDescending = true
        };

        var query = new GetAllFuelPurchasesQuery(parameters);
        SetupValidValidation(query);

        var user = new User
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
            VehicleInspections = [],
            MaintenanceHistories = [],
            IssueAttachments = []
        };

        var vehicleGroup = new VehicleGroup
        {
            ID = 2,
            Name = "Group 1",
            Description = "Test Group",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        // Create Vehicle entities (what the repository returns)
        var vehicleCorolla = new Vehicle
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
            VehicleGroupID = 1,
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
            VehicleInspections = []
        };

        var vehicleCamry = new Vehicle
        {
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
            VehicleInspections = []
        };

        // Create FuelPurchases entities (what the repository returns)
        var fuelPurchasesCorolla = new FuelPurchase
        {
            VehicleId = vehicleCorolla.ID,
            PurchasedByUserId = user.Id,
            PurchaseDate = new DateTime(2020, 1, 1),
            OdometerReading = vehicleCorolla.Mileage,
            Volume = 2,
            PricePerUnit = 2,
            TotalCost = 4,
            FuelStation = "Station1",
            ReceiptNumber = "1",
            Vehicle = vehicleCorolla,
            User = user,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ID = 1,
        };

        var fuelPurchasesCamry = new FuelPurchase
        {
            VehicleId = vehicleCamry.ID,
            PurchasedByUserId = user.Id,
            PurchaseDate = new DateTime(2020, 1, 1),
            OdometerReading = vehicleCamry.Mileage,
            Volume = 3,
            PricePerUnit = 8,
            TotalCost = 24,
            FuelStation = "Station2",
            ReceiptNumber = "2",
            Vehicle = vehicleCamry,
            User = user,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ID = 2,
        };

        var expectedFuelPurchasesEntities = new List<FuelPurchase>
        {
            fuelPurchasesCamry,
            fuelPurchasesCorolla

        };

        var pagedFuelPurchasesEntities = new PagedResult<FuelPurchase>
        {
            Items = expectedFuelPurchasesEntities,
            TotalCount = 25,
            PageNumber = 1,
            PageSize = 5
        };

        _mockFuelPurchasesRepository.Setup(repo => repo.GetAllFuelPurchasesPagedAsync(parameters))
            .ReturnsAsync(pagedFuelPurchasesEntities);

        // When
        var result = await _getAllFuelPurchasesQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.IsType<PagedResult<GetAllFuelPurchasesDTO>>(result);

        Assert.Equal(25, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(5, result.TotalPages); // 25 / 5 = 5 pages
        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);

        // Check items
        Assert.Equal(2, result.Items.Count);

        var firstFuelPurchases = result.Items[0];
        Assert.Equal(2, firstFuelPurchases.ID);
        Assert.Equal(vehicleCamry.ID, firstFuelPurchases.VehicleId);
        Assert.Equal("2", firstFuelPurchases.ReceiptNumber);
        Assert.Equal("Station2", firstFuelPurchases.FuelStation);

        var secondFuelPurchases = result.Items[1];
        Assert.Equal(1, secondFuelPurchases.ID);
        Assert.Equal(vehicleCorolla.ID, secondFuelPurchases.VehicleId);
        Assert.Equal("1", secondFuelPurchases.ReceiptNumber);
        Assert.Equal("Station1", secondFuelPurchases.FuelStation);


        // Verify repository was called with correct parameters
        _mockFuelPurchasesRepository.Verify(r => r.GetAllFuelPurchasesPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task GetAllFuelPurchasesQueryould_Return_Empty_Result_When_No_FuelPurchasess()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "NonExistentBrand"
        };

        var query = new GetAllFuelPurchasesQuery(parameters);

        SetupValidValidation(query);
        var emptyPagedResult = new PagedResult<FuelPurchase>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _mockFuelPurchasesRepository.Setup(repo => repo.GetAllFuelPurchasesPagedAsync(parameters))
            .ReturnsAsync(emptyPagedResult);

        // When
        var result = await _getAllFuelPurchasesQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);

        // Verify repository was called with correct parameters
        _mockFuelPurchasesRepository.Verify(r => r.GetAllFuelPurchasesPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task GetAllFuelPurchasesQueryould_Handle_Different_Page_Sizes()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 2,
            PageSize = 3
        };

        var query = new GetAllFuelPurchasesQuery(parameters);
        SetupValidValidation(query);

        var pagedResult = new PagedResult<FuelPurchase>
        {
            Items = [], // Empty for simplicity
            TotalCount = 10,
            PageNumber = 2,
            PageSize = 3
        };

        _mockFuelPurchasesRepository.Setup(repo => repo.GetAllFuelPurchasesPagedAsync(parameters))
            .ReturnsAsync(pagedResult);
        // When
        var result = await _getAllFuelPurchasesQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(4, result.TotalPages); // 10 / 3 = 4 pages (rounded up)
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockFuelPurchasesRepository.Verify(r => r.GetAllFuelPurchasesPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task GetAllFuelPurchasesQueryould_Handle_Last_Page()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 3,
            PageSize = 5
        };

        var query = new GetAllFuelPurchasesQuery(parameters);
        SetupValidValidation(query);

        var pagedResult = new PagedResult<FuelPurchase>
        {
            Items = [],
            TotalCount = 12,
            PageNumber = 3,
            PageSize = 5
        };

        _mockFuelPurchasesRepository.Setup(r => r.GetAllFuelPurchasesPagedAsync(parameters))
                              .ReturnsAsync(pagedResult);

        // When
        var result = await _getAllFuelPurchasesQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(3, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockFuelPurchasesRepository.Verify(r => r.GetAllFuelPurchasesPagedAsync(parameters), Times.Once);
    }
}
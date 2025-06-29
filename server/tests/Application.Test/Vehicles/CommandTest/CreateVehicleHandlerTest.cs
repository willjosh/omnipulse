using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Vehicle.Command.CreateVehicle;
using Application.Features.Vehicles.Command.CreateVehicle;
using Application.MappingProfiles;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace Application.Test.Vehicles.CommandTest;

public class CreateVehicleHandlerTest
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly CreateVehicleCommandHandler _createVehicleCommandHandler;
    private readonly Mock<IAppLogger<CreateVehicleCommandHandler>> _mockLogger;

    public CreateVehicleHandlerTest()
    {
        _mockVehicleRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VehicleMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _createVehicleCommandHandler = new(_mockVehicleRepository.Object, mapper, _mockLogger.Object);
    }

    [Fact(Skip = "No Implementation yet")]
    public async Task Handler_Should_Return_VehicleID_On_Success()
    {
        // Given 
        var CreateVehicleCommand = new CreateVehicleCommand(
            Name: "James The Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1007",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 0,
            Trim: "JMS",
            Mileage: 300000,
            EngineHours: 1000,
            FuelCapacity: 10000,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 20000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Kuala Lumpur"
        );

        var ExpectedVehicle = new Vehicle()
        {
            ID = 123,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = CreateVehicleCommand.Name,
            Make = CreateVehicleCommand.Make,
            Model = CreateVehicleCommand.Model,
            Year = CreateVehicleCommand.Year,
            VIN = CreateVehicleCommand.VIN,
            LicensePlate = CreateVehicleCommand.LicensePlate,
            LicensePlateExpirationDate = CreateVehicleCommand.LicensePlateExpirationDate,
            VehicleType = CreateVehicleCommand.VehicleType,
            VehicleGroupID = CreateVehicleCommand.VehicleGroupID,
            Trim = CreateVehicleCommand.Trim,
            Mileage = CreateVehicleCommand.Mileage,
            EngineHours = CreateVehicleCommand.EngineHours,
            FuelCapacity = CreateVehicleCommand.FuelCapacity,
            FuelType = CreateVehicleCommand.FuelType,
            PurchaseDate = CreateVehicleCommand.PurchaseDate,
            PurchasePrice = CreateVehicleCommand.PurchasePrice,
            Status = CreateVehicleCommand.Status,
            Location = CreateVehicleCommand.Location,
            VehicleGroup = It.IsAny<VehicleGroup>(),
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        _mockVehicleRepository.Setup(r => r.AddAsync(It.IsAny<Vehicle>())).ReturnsAsync(ExpectedVehicle);
        _mockVehicleRepository
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // When
        var result = await _createVehicleCommandHandler.Handle(CreateVehicleCommand, CancellationToken.None);

        // Then
        Assert.Equal(ExpectedVehicle.ID, result);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), times: Times.Once);

        // Verify repository was called with correct vehicle
        _mockVehicleRepository.Verify(
            r => r.AddAsync(It.Is<Vehicle>(v =>
                v.Name == ExpectedVehicle.Name &&
                v.Make == ExpectedVehicle.Make &&
                v.VIN == ExpectedVehicle.VIN &&
                v.ID == 0)),
            Times.Once);
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_DuplicateEntityException_On_Duplicate_VIN()
    {
        // Given 
        var CreateVehicleCommand = new CreateVehicleCommand(
            Name: "James The Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1007",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 0,
            Trim: "JMS",
            Mileage: 300000,
            EngineHours: 1000,
            FuelCapacity: 10000,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 20000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Kuala Lumpur"
        );

        _mockVehicleRepository.Setup(r => r.VinExistAsync(CreateVehicleCommand.VIN)).ReturnsAsync(true);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(
            async () => await _createVehicleCommandHandler.Handle(CreateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), times: Times.Never);
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_BadRequestException_On_NegativeYear()
    {
        // Given 
        var CreateVehicleCommand = new CreateVehicleCommand(
            Name: "James The Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: -1,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1007",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 0,
            Trim: "JMS",
            Mileage: 300000,
            EngineHours: 1000,
            FuelCapacity: 10000,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 20000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Kuala Lumpur"
        );

        // When && Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _createVehicleCommandHandler.Handle(CreateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), times: Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), times: Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData("AAAAAAAAAAAAAAAA")]
    [InlineData("AAAAAAAAAAAAAAAAAA")]
    public async Task Handler_Should_Return_BadRequestException_On_InvalidVIN(string VIN)
    {
        // Given 
        var CreateVehicleCommand = new CreateVehicleCommand(
            Name: "James The Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: VIN,
            LicensePlate: "BM1007",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 0,
            Trim: "JMS",
            Mileage: 300000,
            EngineHours: 1000,
            FuelCapacity: 10000,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 20000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Kuala Lumpur"
        );

        // When && Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _createVehicleCommandHandler.Handle(CreateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), times: Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), times: Times.Never);
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_BadRequestException_On_ExceedMaxLengthName()
    {

        // Given 
        var nameExceedingLimit = new string('A', 51); // 51 characters - exceeds 50 limit

        var CreateVehicleCommand = new CreateVehicleCommand(
            Name: nameExceedingLimit,
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1007",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 0,
            Trim: "JMS",
            Mileage: 300000,
            EngineHours: 1000,
            FuelCapacity: 10000,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 20000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Kuala Lumpur"
        );

        // When && Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _createVehicleCommandHandler.Handle(CreateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), times: Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), times: Times.Never);
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_BadRequestException_On_ExceedMaxLengthMake()
    {
        // Given 
        var MakeExceedingLimit = new string('A', 31); // 31 characters - exceeds 30 limit

        var CreateVehicleCommand = new CreateVehicleCommand(
            Name: "James the Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1007",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 0,
            Trim: "JMS",
            Mileage: 300000,
            EngineHours: 1000,
            FuelCapacity: 10000,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 20000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Kuala Lumpur"
        );

        // When && Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _createVehicleCommandHandler.Handle(CreateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), times: Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), times: Times.Never);
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_BadRequestException_On_ExceedMaxLengthModel()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_BadRequestException_On_ExceedMaxLengthTrim()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_BadRequestException_On_ExceedMaxLengthLocation()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_BadRequestException_On_ExceedMaxLengthVIN()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_BadRequestException_On_ExceedMaxLengthLicensePlate()
    {
        throw new NotImplementedException();
    }


    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_BadRequestException_On_LicenseExpirationDateBeforeOrEqualYear()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_BadRequestException_On_InvalidVehicleType()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_NotFoundException_On_InvalidGroupID()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_NotFoundException_On_InvalidTechnicianID()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_NotFoundException_On_NegativeMileage()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_NotFoundException_On_NegativeEngineHours()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_NotFoundException_On_NegativeFuelCapacity()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_NotFoundException_On_InvalidFuelType()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_NotFoundException_On_NegativePurchasePrice()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Return_NotFoundException_On_InvalidStatus()
    {
        throw new NotImplementedException();
    }
}

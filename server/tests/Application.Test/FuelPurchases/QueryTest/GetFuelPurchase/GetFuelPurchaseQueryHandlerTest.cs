using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.FuelPurchases.Query.GetFuelPurchase;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using Moq;

namespace Application.Test.FuelPurchases.QueryTest.GetFuelPurchase;

public class GetFuelPurchaseQueryHandlerTest
{
    private readonly GetFuelPurchaseQueryHandler _handler;
    private readonly Mock<IFuelPurchaseRepository> _mockRepo = new();
    private readonly Mock<IValidator<GetFuelPurchaseQuery>> _mockValidator = new();
    private readonly Mock<IAppLogger<GetFuelPurchaseQueryHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    public GetFuelPurchaseQueryHandlerTest()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<FuelPurchaseMappingProfile>());
        _mapper = cfg.CreateMapper();
        _handler = new GetFuelPurchaseQueryHandler(_mockRepo.Object, _mockValidator.Object, _mockLogger.Object, _mapper);
    }

    [Fact]
    public async Task Handler_Should_Return_DTO_On_Success()
    {
        // ===== Arrange =====
        var entity = new FuelPurchase
        {
            ID = 5,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleId = 10,
            PurchasedByUserId = Guid.NewGuid().ToString(),
            PurchaseDate = DateTime.UtcNow.Date,
            OdometerReading = 12345.6,
            Volume = 45.5,
            PricePerUnit = 3.25m,
            TotalCost = 148.0m,
            FuelStation = "Shell",
            ReceiptNumber = "RCPT-1",
            Vehicle = null!,
            User = null!
        };
        _mockRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(entity);
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<GetFuelPurchaseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // ===== Act =====
        var dto = await _handler.Handle(new GetFuelPurchaseQuery(5), default);

        // ===== Assert =====
        Assert.Equal(entity.ID, dto.ID);
        Assert.Equal(entity.VehicleId, dto.VehicleId);
        Assert.Equal(entity.PurchasedByUserId, dto.PurchasedByUserId);
        Assert.Equal(entity.TotalCost, dto.TotalCost);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequest_On_Invalid_ID()
    {
        // ===== Arrange =====
        var query = new GetFuelPurchaseQuery(0);
        _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult([
                new FluentValidation.Results.ValidationFailure(nameof(GetFuelPurchaseQuery.FuelPurchaseID), "Invalid")
            ]));

        // ===== Act & Assert =====
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(query, default));
    }

    [Fact]
    public async Task Handler_Should_Throw_NotFound_When_Missing()
    {
        // ===== Arrange =====
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FuelPurchase?)null);
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<GetFuelPurchaseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // ===== Act & Assert =====
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(new GetFuelPurchaseQuery(999), default));
    }
}
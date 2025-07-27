using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.InventoryItemLocations.Query.GetAllInventoryItemLocation;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using Moq;

namespace Application.Test.InventoryItemLocations.QueryTest.GetAllInventoryItemLocation;

public class GetAllInventoryItemLocationQueryHandlerTest
{
    private readonly Mock<IInventoryItemLocationRepository> _mockRepository;
    private readonly Mock<IAppLogger<GetAllInventoryItemLocationQueryHandler>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly GetAllInventoryItemLocationQueryHandler _handler;

    public GetAllInventoryItemLocationQueryHandlerTest()
    {
        _mockRepository = new Mock<IInventoryItemLocationRepository>();
        _mockLogger = new Mock<IAppLogger<GetAllInventoryItemLocationQueryHandler>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<InventoryItemLocationMappingProfile>());
        _mapper = config.CreateMapper();
        _handler = new GetAllInventoryItemLocationQueryHandler(
            _mockRepository.Object,
            _mapper,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task Handler_Should_Return_All_DTOs_When_Repository_Has_Entities()
    {
        // Arrange
        var entities = new List<InventoryItemLocation>
        {
            new InventoryItemLocation
            {
                ID = 1,
                LocationName = "Warehouse A",
                Address = "123 Main St",
                Longitude = 100.0,
                Latitude = 10.0,
                Capacity = 50,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Inventories = new List<Domain.Entities.Inventory>()
            },
            new InventoryItemLocation
            {
                ID = 2,
                LocationName = "Warehouse B",
                Address = "456 Side St",
                Longitude = 101.0,
                Latitude = 11.0,
                Capacity = 75,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Inventories = new List<Domain.Entities.Inventory>()
            }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        var query = new GetAllInventoryItemLocationQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Collection(result,
            dto =>
            {
                Assert.Equal(1, dto.ID);
                Assert.Equal("Warehouse A", dto.LocationName);
                Assert.Equal("123 Main St", dto.Address);
                Assert.Equal(100.0, dto.Longitude);
                Assert.Equal(10.0, dto.Latitude);
                Assert.Equal(50, dto.Capacity);
                Assert.True(dto.IsActive);
            },
            dto =>
            {
                Assert.Equal(2, dto.ID);
                Assert.Equal("Warehouse B", dto.LocationName);
                Assert.Equal("456 Side St", dto.Address);
                Assert.Equal(101.0, dto.Longitude);
                Assert.Equal(11.0, dto.Latitude);
                Assert.Equal(75, dto.Capacity);
                Assert.False(dto.IsActive);
            }
        );
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_List_When_Repository_Has_No_Entities()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<InventoryItemLocation>());
        var query = new GetAllInventoryItemLocationQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }
}
using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.VehicleGroups.Query.GetAllVehicleGroup;
using Moq;
using AutoMapper;
using Application.MappingProfiles;
using Application.Models;
using Domain.Entities;
using Application.Models.PaginationModels;
using FluentValidation;

namespace Application.Test.VehicleGroups.QueryTest.GetAllVehicleGroup;

public class GetAllVehicleGroupQueryHandlerTest
{

    private readonly Mock<IVehicleGroupRepository> _mockVehicleGroupRepository;
    private readonly GetAllVehicleGroupQueryHandler _getAllVehicleGroupQueryHandler;
    private readonly Mock<IAppLogger<GetAllVehicleGroupQueryHandler>> _mockLogger;
    private readonly IMapper _mapper;

    private readonly Mock<IValidator<GetAllVehicleGroupQuery>> _mockValidator;
    public GetAllVehicleGroupQueryHandlerTest()
    {
        _mockVehicleGroupRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VehicleGroupMappingProfile>();
        });

        _mapper = config.CreateMapper();

        _getAllVehicleGroupQueryHandler = new GetAllVehicleGroupQueryHandler(
            _mockVehicleGroupRepository.Object,
            _mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    // Helper method to set up valid validation result
    private void SetupValidValidation(GetAllVehicleGroupQuery query)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(validResult);
    }

    // Helper method to set up validation failure
    private void SetupInvalidValidation(GetAllVehicleGroupQuery query, string propertyName = "Parameters.PageNumber", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_GetAllVehicleGroupDTO_On_Success()
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

        var query = new GetAllVehicleGroupQuery(parameters);
        SetupValidValidation(query);

        // Create VehicleGroup entities (what the repository returns)
        var vehicleGroupToyota = new VehicleGroup
        {
            ID = 123,
            Name = "Toyota",
            Description = "Toyota vehicles",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var vehicleGroupHonda = new VehicleGroup
        {
            ID = 456,
            Name = "Honda",
            Description = "Honda vehicles",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var expectedVehicleGroupEntities = new List<VehicleGroup>
        {
            vehicleGroupToyota,
            vehicleGroupHonda
        };

        var pagedVehicleGroupEntities = new PagedResult<VehicleGroup>
        {
            Items = expectedVehicleGroupEntities,
            TotalCount = 25,
            PageNumber = 1,
            PageSize = 5
        };

        _mockVehicleGroupRepository.Setup(repo => repo.GetAllVehicleGroupsPagedAsync(parameters))
            .ReturnsAsync(pagedVehicleGroupEntities);

        // When
        var result = await _getAllVehicleGroupQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.IsType<PagedResult<GetAllVehicleGroupDTO>>(result);

        Assert.Equal(25, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(5, result.TotalPages); // 25 / 5 = 5 pages
        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);

        // Check items
        Assert.Equal(2, result.Items.Count);

        var firstVehicleGroup = result.Items[0];
        Assert.Equal(123, firstVehicleGroup.ID);
        Assert.Equal("Toyota", firstVehicleGroup.Name);
        Assert.Equal("Toyota vehicles", firstVehicleGroup.Description);
        Assert.True(firstVehicleGroup.IsActive);

        var secondVehicleGroup = result.Items[1];
        Assert.Equal(456, secondVehicleGroup.ID);
        Assert.Equal("Honda", secondVehicleGroup.Name);
        Assert.Equal("Honda vehicles", secondVehicleGroup.Description);
        Assert.True(secondVehicleGroup.IsActive);

        // Verify repository was called with correct parameters
        _mockVehicleGroupRepository.Verify(r => r.GetAllVehicleGroupsPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_VehicleGroups()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "NonExistentBrand"
        };

        var query = new GetAllVehicleGroupQuery(parameters);

        SetupValidValidation(query);
        var emptyPagedResult = new PagedResult<VehicleGroup>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _mockVehicleGroupRepository.Setup(repo => repo.GetAllVehicleGroupsPagedAsync(parameters))
            .ReturnsAsync(emptyPagedResult);

        // When
        var result = await _getAllVehicleGroupQueryHandler.Handle(query, CancellationToken.None);

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
        _mockVehicleGroupRepository.Verify(r => r.GetAllVehicleGroupsPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Different_Page_Sizes()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 3
        };

        var query = new GetAllVehicleGroupQuery(parameters);

        SetupValidValidation(query);
        var vehicleGroupEntities = new List<VehicleGroup>
        {
            new VehicleGroup { ID = 1, Name = "Toyota", Description = "Toyota vehicles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new VehicleGroup { ID = 2, Name = "Honda", Description = "Honda vehicles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new VehicleGroup { ID = 3, Name = "Ford", Description = "Ford vehicles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
        };

        var pagedVehicleGroupEntities = new PagedResult<VehicleGroup>
        {
            Items = vehicleGroupEntities,
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 3
        };

        _mockVehicleGroupRepository.Setup(repo => repo.GetAllVehicleGroupsPagedAsync(parameters))
            .ReturnsAsync(pagedVehicleGroupEntities);

        // When
        var result = await _getAllVehicleGroupQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(1, result.TotalPages); // 3 / 3 = 1 page
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);

        // Check items
        Assert.Equal(3, result.Items.Count);

        var firstVehicleGroup = result.Items[0];
        Assert.Equal(1, firstVehicleGroup.ID);
        Assert.Equal("Toyota", firstVehicleGroup.Name);
        Assert.Equal("Toyota vehicles", firstVehicleGroup.Description);
        Assert.True(firstVehicleGroup.IsActive);
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

        var query = new GetAllVehicleGroupQuery(parameters);

        SetupValidValidation(query);
        var vehicleGroupEntities = new List<VehicleGroup>
        {
            new VehicleGroup { ID = 1, Name = "Toyota", Description = "Toyota vehicles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new VehicleGroup { ID = 2, Name = "Honda", Description = "Honda vehicles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new VehicleGroup { ID = 3, Name = "Ford", Description = "Ford vehicles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
        };

        var pagedVehicleGroupEntities = new PagedResult<VehicleGroup>
        {
            Items = vehicleGroupEntities,
            TotalCount = 12,
            PageNumber = 3,
            PageSize = 5
        };

        _mockVehicleGroupRepository.Setup(repo => repo.GetAllVehicleGroupsPagedAsync(parameters))
            .ReturnsAsync(pagedVehicleGroupEntities);

        // When
        var result = await _getAllVehicleGroupQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(3, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(3, result.TotalPages); // 12 / 5 = 3 pages (rounded up)
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage); // Last page
    }
}

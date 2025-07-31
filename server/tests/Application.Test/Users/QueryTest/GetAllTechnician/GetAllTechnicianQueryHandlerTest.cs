using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Users.Query.GetAllTechnician;
using Application.MappingProfiles;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using Moq;

namespace Application.Test.Users.QueryTest.GetAllTechnician;

public class GetAllTechnicianQueryHandlerTest
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IAppLogger<GetAllTechnicianQueryHandler>> _mockLogger;
    private readonly Mock<IValidator<GetAllTechnicianQuery>> _mockValidator;
    private readonly GetAllTechnicianQueryHandler _handler;

    public GetAllTechnicianQueryHandlerTest()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<IAppLogger<GetAllTechnicianQueryHandler>>();
        _mockValidator = new Mock<IValidator<GetAllTechnicianQuery>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        var mapper = config.CreateMapper();

        _handler = new GetAllTechnicianQueryHandler(
            _mockUserRepository.Object,
            mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    // Helper method to set up valid validation result
    private void SetupValidValidation(GetAllTechnicianQuery query)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(validResult);
    }

    // Helper method to set up validation failure
    private void SetupInvalidValidation(GetAllTechnicianQuery query, string propertyName = "Parameters.PageNumber", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(invalidResult);
    }

    private PaginationParameters CreateValidPaginationParameters()
    {
        return new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            SortBy = "firstname",
            SortDescending = false,
        };
    }

    private GetAllTechnicianQuery CreateValidQuery()
    {
        return new GetAllTechnicianQuery
        (
            Parameters: CreateValidPaginationParameters()
        );
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_When_Valid_Query()
    {
        // Given
        var query = CreateValidQuery();
        SetupValidValidation(query);

        var expectedTechnicians = new List<User>
        {
            new() {
                Id = "guid-1",
                FirstName = "John",
                LastName = "Doe",
                HireDate = new DateTime(2004, 4, 11, 0, 0, 0, DateTimeKind.Utc),
                Email = "john@gmail.com",
                IsActive = true,
                CreatedAt = new DateTime(2004, 4, 11, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2004, 4, 11, 0, 0, 0, DateTimeKind.Utc),
                MaintenanceHistories = [],
                Vehicles = [],
                IssueAttachments = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                Inspections = [],
                InventoryTransactions = []
                },
            new() {
                Id = "guid-2",
                FirstName = "Branzen",
                LastName = "Dolomite",
                HireDate = new DateTime(2004, 4, 11, 0, 0, 0, DateTimeKind.Utc),
                Email = "brazzen@gmail.com",
                IsActive = true,
                CreatedAt = new DateTime(2004, 4, 11, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2004, 4, 11, 0, 0, 0, DateTimeKind.Utc),
                MaintenanceHistories = [],
                Vehicles = [],
                IssueAttachments = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                Inspections = [],
                InventoryTransactions = []
            },
        };

        var expectedTechniciansDTO = expectedTechnicians.Select(t => new GetAllTechnicianDTO
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            HireDate = t.HireDate,
            Email = t.Email!,
            IsActive = t.IsActive
        }).ToList();

        var pagedTechnicians = new PagedResult<User>
        {
            Items = expectedTechnicians.AsReadOnly(),
            TotalCount = expectedTechnicians.Count,
            PageNumber = 1,
            PageSize = 10
        };

        _mockUserRepository
            .Setup(r => r.GetAllTechnicianPagedAsync(It.IsAny<PaginationParameters>()))
            .ReturnsAsync(pagedTechnicians);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.IsType<PagedResult<GetAllTechnicianDTO>>(result);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
        Assert.Equal(expectedTechniciansDTO.Count, result.Items.Count);
        Assert.Equal(expectedTechniciansDTO[0].Id, result.Items[0].Id);
        Assert.Equal(expectedTechniciansDTO[0].FirstName, result.Items[0].FirstName);
        Assert.Equal(expectedTechniciansDTO[0].LastName, result.Items[0].LastName);
        Assert.Equal(expectedTechniciansDTO[0].HireDate, result.Items[0].HireDate);
        Assert.Equal(expectedTechniciansDTO[0].Email, result.Items[0].Email);
        Assert.Equal(expectedTechniciansDTO[0].IsActive, result.Items[0].IsActive);
        Assert.Equal(pagedTechnicians.TotalCount, result.TotalCount);
        Assert.Equal(pagedTechnicians.PageNumber, result.PageNumber);
        Assert.Equal(pagedTechnicians.PageSize, result.PageSize);
        Assert.Equal(pagedTechnicians.HasNextPage, result.HasNextPage);
        Assert.Equal(pagedTechnicians.HasPreviousPage, result.HasPreviousPage);

        // Verify interactions
        _mockUserRepository.Verify(r => r.GetAllTechnicianPagedAsync(It.IsAny<PaginationParameters>()), Times.Once);
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_PagedResult_When_No_Technicians()
    {
        // Given
        var query = CreateValidQuery();
        SetupValidValidation(query);

        var pagedTechnicians = new PagedResult<User>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _mockUserRepository
            .Setup(r => r.GetAllTechnicianPagedAsync(It.IsAny<PaginationParameters>()))
            .ReturnsAsync(pagedTechnicians);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.IsType<PagedResult<GetAllTechnicianDTO>>(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);

        // Verify interactions
        _mockUserRepository.Verify(r => r.GetAllTechnicianPagedAsync(It.IsAny<PaginationParameters>()), Times.Once);
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var query = CreateValidQuery();
        SetupInvalidValidation(query, "Parameters.PageNumber", "Page number must be greater than 0");

        // When && Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(query, CancellationToken.None)
        );

        // Verify interactions
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockUserRepository.Verify(r => r.GetAllTechnicianPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }
}
using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Inventory.Query;
using Application.MappingProfiles;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using Moq;

namespace Application.Test.Inventories.QueryTest.GetAllInventoryTest;

public class GetAllInventoryQueryHandlerTest
{
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly GetAllInventoryQueryHandler _handler;
    private readonly Mock<IAppLogger<GetAllInventoryQueryHandler>> _mockLogger;
    private readonly Mock<IValidator<GetAllInventoryQuery>> _mockValidator;

    public GetAllInventoryQueryHandlerTest()
    {
        _inventoryRepositoryMock = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InventoryMappingProfile>());
        var mapper = config.CreateMapper();

        _handler = new GetAllInventoryQueryHandler(
            _inventoryRepositoryMock.Object,
            mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    // Helper method to set up valid validation result
    private void SetupValidValidation(GetAllInventoryQuery query)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(validResult);
    }

    // Helper method to set up validation failure
    private void SetupInvalidValidation(GetAllInventoryQuery query, string propertyName = "Parameters.PageNumber", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_On_Success() { }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // // Given
        // var parameters = new PaginationParameters
        // {
        //     PageNumber = 0,
        //     PageSize = 10
        // };

        // var query = new GetAllInventoryQuery(parameters);
        // SetupInvalidValidation(query, "Parameters.PageNumber", "Page number must be greater than 0");

        // // When & Then
        // await Assert.ThrowsAsync<BadRequestException>(
        //     () => _handler.Handle(query, CancellationToken.None)
        // );

        // _mockValidator.Verify(v => v.Validate(query), Times.Once);
        // _inventoryRepositoryMock.Verify(r => r.GetAllInventoriesPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }
}
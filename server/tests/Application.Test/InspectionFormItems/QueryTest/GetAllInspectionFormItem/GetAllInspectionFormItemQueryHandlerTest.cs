using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InspectionFormItems.Query.GetAllInspectionFormItem;
using Application.MappingProfiles;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.InspectionFormItems.QueryTest.GetAllInspectionFormItem;

public class GetAllInspectionFormItemQueryHandlerTest
{
    private readonly GetAllInspectionFormItemQueryHandler _queryHandler;
    private readonly Mock<IInspectionFormItemRepository> _mockInspectionFormItemRepository = new();
    private readonly Mock<IInspectionFormRepository> _mockInspectionFormRepository = new();
    private readonly Mock<IValidator<GetAllInspectionFormItemQuery>> _mockValidator = new();
    private readonly Mock<IAppLogger<GetAllInspectionFormItemQueryHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public GetAllInspectionFormItemQueryHandlerTest()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<InspectionFormItemMappingProfile>());
        _mapper = config.CreateMapper();
        _queryHandler = new GetAllInspectionFormItemQueryHandler(
            _mockInspectionFormItemRepository.Object,
            _mockInspectionFormRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper);
    }

    private static GetAllInspectionFormItemQuery CreateValidQuery(
        int inspectionFormID = 1,
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null,
        string? sortBy = null,
        bool sortDescending = false)
    {
        var parameters = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Search = search,
            SortBy = sortBy,
            SortDescending = sortDescending
        };
        return new GetAllInspectionFormItemQuery(inspectionFormID, parameters);
    }

    private static InspectionForm CreateInspectionFormEntity(
        int id = 1,
        string title = "Vehicle Safety Inspection",
        bool isActive = true)
    {
        return new InspectionForm
        {
            ID = id,
            Title = title,
            Description = "Test inspection form",
            IsActive = isActive,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inspections = [],
            InspectionFormItems = []
        };
    }

    private static List<InspectionFormItem> CreateInspectionFormItemList(int count = 3)
    {
        var items = new List<InspectionFormItem>();
        for (int i = 1; i <= count; i++)
        {
            items.Add(new InspectionFormItem
            {
                ID = i,
                InspectionFormID = 1,
                ItemLabel = $"Check Item {i}",
                ItemDescription = $"Description for item {i}",
                ItemInstructions = $"Instructions for item {i}",
                InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
                IsRequired = i % 2 == 1, // Alternate required/optional
                IsActive = true,
                CreatedAt = FixedDate.AddMinutes(i),
                UpdatedAt = FixedDate.AddMinutes(i + 10),
                InspectionForm = null!
            });
        }
        return items;
    }

    private void SetupValidValidation(GetAllInspectionFormItemQuery query)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.Validate(query))
            .Returns(validResult);
    }

    private void SetupInvalidValidation(GetAllInspectionFormItemQuery query, string propertyName = "Parameters", string errorMessage = "Invalid Parameters")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.Validate(query))
            .Returns(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_When_Successful()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        var inspectionForm = CreateInspectionFormEntity();
        var items = CreateInspectionFormItemList(3);
        var pagedResult = new PagedResult<InspectionFormItem>
        {
            Items = items,
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(query.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.GetAllByInspectionFormIdPagedAsync(query.InspectionFormID, query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);

        var firstDto = result.Items.First();
        Assert.Equal(1, firstDto.ID);
        Assert.Equal("Check Item 1", firstDto.ItemLabel);
        Assert.True(firstDto.IsRequired);

        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(query.InspectionFormID), Times.Once);
        _mockInspectionFormItemRepository.Verify(r => r.GetAllByInspectionFormIdPagedAsync(query.InspectionFormID, query.Parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_Items()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        var inspectionForm = CreateInspectionFormEntity();
        var pagedResult = new PagedResult<InspectionFormItem>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(query.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.GetAllByInspectionFormIdPagedAsync(query.InspectionFormID, query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_InspectionForm_Not_Found()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(query.InspectionFormID))
            .ReturnsAsync((InspectionForm?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _queryHandler.Handle(query, CancellationToken.None));

        Assert.Equal(nameof(InspectionForm), exception.EntityName);
        Assert.Equal(nameof(InspectionForm.ID), exception.PropertyName);
        Assert.Equal(query.InspectionFormID.ToString(), exception.PropertyValue);

        _mockInspectionFormItemRepository.Verify(r => r.GetAllByInspectionFormIdPagedAsync(It.IsAny<int>(), It.IsAny<PaginationParameters>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var query = CreateValidQuery(pageNumber: -1); // Invalid page number
        SetupInvalidValidation(query);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _queryHandler.Handle(query, CancellationToken.None));

        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockInspectionFormItemRepository.Verify(r => r.GetAllByInspectionFormIdPagedAsync(It.IsAny<int>(), It.IsAny<PaginationParameters>()), Times.Never);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 10)]
    [InlineData(3, 25)]
    public async Task Handler_Should_Handle_Different_Pagination_Parameters(int pageNumber, int pageSize)
    {
        // Arrange
        var query = CreateValidQuery(pageNumber: pageNumber, pageSize: pageSize);
        SetupValidValidation(query);
        var inspectionForm = CreateInspectionFormEntity();
        var items = CreateInspectionFormItemList(2);
        var pagedResult = new PagedResult<InspectionFormItem>
        {
            Items = items,
            TotalCount = 50, // Simulate larger dataset
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(query.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.GetAllByInspectionFormIdPagedAsync(query.InspectionFormID, query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(2, result.TotalCount); // 2 active items from CreateInspectionFormItemList(2)
    }

    [Fact]
    public async Task Handler_Should_Handle_Search_Parameters()
    {
        // Arrange
        var query = CreateValidQuery(search: "engine");
        SetupValidValidation(query);
        var inspectionForm = CreateInspectionFormEntity();
        var items = CreateInspectionFormItemList(1);
        var pagedResult = new PagedResult<InspectionFormItem>
        {
            Items = items,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(query.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.GetAllByInspectionFormIdPagedAsync(query.InspectionFormID, query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        _mockInspectionFormItemRepository.Verify(r => r.GetAllByInspectionFormIdPagedAsync(
            query.InspectionFormID,
            It.Is<PaginationParameters>(p => p.Search == "engine")), Times.Once);
    }

    [Theory]
    [InlineData("itemlabel", false)]
    [InlineData("itemlabel", true)]
    [InlineData("createdat", false)]
    [InlineData("updatedat", true)]
    public async Task Handler_Should_Handle_Sorting_Parameters(string sortBy, bool sortDescending)
    {
        // Arrange
        var query = CreateValidQuery(sortBy: sortBy, sortDescending: sortDescending);
        SetupValidValidation(query);
        var inspectionForm = CreateInspectionFormEntity();
        var items = CreateInspectionFormItemList(2);
        var pagedResult = new PagedResult<InspectionFormItem>
        {
            Items = items,
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 10
        };

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(query.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.GetAllByInspectionFormIdPagedAsync(query.InspectionFormID, query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        _mockInspectionFormItemRepository.Verify(r => r.GetAllByInspectionFormIdPagedAsync(
            query.InspectionFormID,
            It.Is<PaginationParameters>(p => p.SortBy == sortBy && p.SortDescending == sortDescending)), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public async Task Handler_Should_Work_With_Different_InspectionForm_IDs(int inspectionFormID)
    {
        // Arrange
        var query = CreateValidQuery(inspectionFormID: inspectionFormID);
        SetupValidValidation(query);
        var inspectionForm = CreateInspectionFormEntity(id: inspectionFormID);
        var items = CreateInspectionFormItemList(1);
        var pagedResult = new PagedResult<InspectionFormItem>
        {
            Items = items,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(inspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.GetAllByInspectionFormIdPagedAsync(inspectionFormID, query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(inspectionFormID), Times.Once);
    }
}
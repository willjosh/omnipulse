using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InspectionForms.Query.GetAllInspectionForm;
using Application.MappingProfiles;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.InspectionForms.QueryTest.GetAllInspectionForm;

public class GetAllInspectionFormQueryHandlerTest
{
    private readonly GetAllInspectionFormQueryHandler _queryHandler;
    private readonly Mock<IInspectionFormRepository> _mockInspectionFormRepository = new();
    private readonly Mock<IValidator<GetAllInspectionFormQuery>> _mockValidator = new();
    private readonly Mock<IAppLogger<GetAllInspectionFormQueryHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public GetAllInspectionFormQueryHandlerTest()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<InspectionFormMappingProfile>());
        _mapper = config.CreateMapper();
        _queryHandler = new GetAllInspectionFormQueryHandler(
            _mockInspectionFormRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper);
    }

    private static GetAllInspectionFormQuery CreateValidQuery(
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
        return new GetAllInspectionFormQuery(parameters);
    }

    private static List<InspectionForm> CreateInspectionFormList(int count = 3)
    {
        var inspectionForms = new List<InspectionForm>();
        for (int i = 1; i <= count; i++)
        {
            // Create mock collections for counting
            var inspections = new List<Inspection>();
            for (int j = 0; j < i * 2; j++) // Different counts for each form
            {
                inspections.Add(null!); // Just for count
            }

            var inspectionFormItems = new List<InspectionFormItem>();
            for (int j = 0; j < i * 3; j++) // Different counts for each form
            {
                inspectionFormItems.Add(new InspectionFormItem
                {
                    ID = j + 1,
                    InspectionFormID = i,
                    ItemLabel = $"Test Item {j + 1}",
                    IsRequired = true,
                    InspectionFormItemTypeEnum = Domain.Entities.Enums.InspectionFormItemTypeEnum.PassFail,
                    IsActive = true, // Active items
                    CreatedAt = FixedDate,
                    UpdatedAt = FixedDate,
                    InspectionForm = null!
                });
            }

            inspectionForms.Add(new InspectionForm
            {
                ID = i,
                Title = $"Inspection Form {i}",
                Description = $"Description for form {i}",
                IsActive = i % 2 == 1, // Alternate active/inactive
                CreatedAt = FixedDate.AddDays(i),
                UpdatedAt = FixedDate.AddDays(i + 10),
                Inspections = inspections,
                InspectionFormItems = inspectionFormItems
            });
        }
        return inspectionForms;
    }

    private void SetupValidValidation(GetAllInspectionFormQuery query)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.Validate(query))
            .Returns(validResult);
    }

    private void SetupInvalidValidation(GetAllInspectionFormQuery query, string propertyName = "Parameters", string errorMessage = "Invalid Parameters")
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
        var inspectionForms = CreateInspectionFormList(3);
        var pagedResult = new PagedResult<InspectionForm>
        {
            Items = inspectionForms,
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInspectionFormRepository.Setup(r => r.GetAllInspectionFormsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert - Only 2 out of 3 forms are active (forms 1 and 3)
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);

        // Verify calculated properties
        var firstDto = result.Items.First();
        Assert.Equal(1, firstDto.ID);
        Assert.Equal("Inspection Form 1", firstDto.Title);
        Assert.Equal(2, firstDto.InspectionCount); // 1 * 2
        Assert.Equal(3, firstDto.InspectionFormItemCount); // 1 * 3

        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetAllInspectionFormsPagedAsync(query.Parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_Items()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        var pagedResult = new PagedResult<InspectionForm>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInspectionFormRepository.Setup(r => r.GetAllInspectionFormsPagedAsync(query.Parameters))
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
    public async Task Handler_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var query = CreateValidQuery(pageNumber: -1); // Invalid page number
        SetupInvalidValidation(query);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _queryHandler.Handle(query, CancellationToken.None));

        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetAllInspectionFormsPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Handle_Null_Collections_Gracefully()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        var inspectionForm = new InspectionForm
        {
            ID = 1,
            Title = "Test Form",
            Description = "Test Description",
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inspections = null!, // Null collection
            InspectionFormItems = null! // Null collection
        };
        var pagedResult = new PagedResult<InspectionForm>
        {
            Items = [inspectionForm],
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInspectionFormRepository.Setup(r => r.GetAllInspectionFormsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        var dto = result.Items.First();
        Assert.Equal(0, dto.InspectionCount);
        Assert.Equal(0, dto.InspectionFormItemCount);
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
        var inspectionForms = CreateInspectionFormList(2);
        var pagedResult = new PagedResult<InspectionForm>
        {
            Items = inspectionForms,
            TotalCount = 50, // Simulate larger dataset
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        _mockInspectionFormRepository.Setup(r => r.GetAllInspectionFormsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(1, result.TotalCount); // Only 1 active form out of 2 test forms
    }

    [Fact]
    public async Task Handler_Should_Handle_Search_Parameters()
    {
        // Arrange
        var query = CreateValidQuery(search: "safety");
        SetupValidValidation(query);
        var inspectionForms = CreateInspectionFormList(1);
        var pagedResult = new PagedResult<InspectionForm>
        {
            Items = inspectionForms,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInspectionFormRepository.Setup(r => r.GetAllInspectionFormsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        _mockInspectionFormRepository.Verify(r => r.GetAllInspectionFormsPagedAsync(
            It.Is<PaginationParameters>(p => p.Search == "safety")), Times.Once);
    }

    [Theory]
    [InlineData("title", false)]
    [InlineData("title", true)]
    [InlineData("createdat", false)]
    [InlineData("updatedat", true)]
    public async Task Handler_Should_Handle_Sorting_Parameters(string sortBy, bool sortDescending)
    {
        // Arrange
        var query = CreateValidQuery(sortBy: sortBy, sortDescending: sortDescending);
        SetupValidValidation(query);
        var inspectionForms = CreateInspectionFormList(2);
        var pagedResult = new PagedResult<InspectionForm>
        {
            Items = inspectionForms,
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInspectionFormRepository.Setup(r => r.GetAllInspectionFormsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert - Only 1 out of 2 forms is active (form 1)
        Assert.NotNull(result);
        Assert.Single(result.Items);
        _mockInspectionFormRepository.Verify(r => r.GetAllInspectionFormsPagedAsync(
            It.Is<PaginationParameters>(p => p.SortBy == sortBy && p.SortDescending == sortDescending)), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Map_All_Properties_Correctly()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        var inspectionForm = new InspectionForm
        {
            ID = 42,
            Title = "Custom Safety Inspection",
            Description = "Custom description for testing",
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate.AddDays(5),
            Inspections = [null!, null!, null!], // 3 items
            InspectionFormItems = [
                new InspectionFormItem { ID = 1, IsActive = true, ItemLabel = "Item 1", IsRequired = true, InspectionFormItemTypeEnum = Domain.Entities.Enums.InspectionFormItemTypeEnum.PassFail, CreatedAt = FixedDate, UpdatedAt = FixedDate, InspectionFormID = 42, InspectionForm = null! },
                new InspectionFormItem { ID = 2, IsActive = true, ItemLabel = "Item 2", IsRequired = true, InspectionFormItemTypeEnum = Domain.Entities.Enums.InspectionFormItemTypeEnum.PassFail, CreatedAt = FixedDate, UpdatedAt = FixedDate, InspectionFormID = 42, InspectionForm = null! },
                new InspectionFormItem { ID = 3, IsActive = true, ItemLabel = "Item 3", IsRequired = true, InspectionFormItemTypeEnum = Domain.Entities.Enums.InspectionFormItemTypeEnum.PassFail, CreatedAt = FixedDate, UpdatedAt = FixedDate, InspectionFormID = 42, InspectionForm = null! },
                new InspectionFormItem { ID = 4, IsActive = true, ItemLabel = "Item 4", IsRequired = true, InspectionFormItemTypeEnum = Domain.Entities.Enums.InspectionFormItemTypeEnum.PassFail, CreatedAt = FixedDate, UpdatedAt = FixedDate, InspectionFormID = 42, InspectionForm = null! },
                new InspectionFormItem { ID = 5, IsActive = true, ItemLabel = "Item 5", IsRequired = true, InspectionFormItemTypeEnum = Domain.Entities.Enums.InspectionFormItemTypeEnum.PassFail, CreatedAt = FixedDate, UpdatedAt = FixedDate, InspectionFormID = 42, InspectionForm = null! }
            ] // 5 active items
        };
        var pagedResult = new PagedResult<InspectionForm>
        {
            Items = [inspectionForm],
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInspectionFormRepository.Setup(r => r.GetAllInspectionFormsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var dto = result.Items.First();
        Assert.Equal(42, dto.ID);
        Assert.Equal("Custom Safety Inspection", dto.Title);
        Assert.Equal("Custom description for testing", dto.Description);
        Assert.True(dto.IsActive);
        Assert.Equal(FixedDate, dto.CreatedAt);
        Assert.Equal(FixedDate.AddDays(5), dto.UpdatedAt);
        Assert.Equal(3, dto.InspectionCount);
        Assert.Equal(5, dto.InspectionFormItemCount);
    }
}
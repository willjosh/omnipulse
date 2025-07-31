using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InspectionForms.Query.GetInspectionForm;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using Moq;

namespace Application.Test.InspectionForms.QueryTest.GetInspectionForm;

public class GetInspectionFormQueryHandlerTest
{
    private readonly GetInspectionFormQueryHandler _queryHandler;
    private readonly Mock<IInspectionFormRepository> _mockInspectionFormRepository = new();
    private readonly Mock<IAppLogger<GetInspectionFormQueryHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public GetInspectionFormQueryHandlerTest()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<InspectionFormMappingProfile>());
        _mapper = config.CreateMapper();
        _queryHandler = new GetInspectionFormQueryHandler(
            _mockInspectionFormRepository.Object,
            _mockLogger.Object,
            _mapper);
    }

    private static GetInspectionFormQuery CreateValidQuery(int inspectionFormID = 1) => new(inspectionFormID);

    private static InspectionForm CreateInspectionFormEntity(
        int id = 1,
        string title = $"Test {nameof(InspectionForm)} Title",
        string? description = $"Test {nameof(InspectionForm)} Description",
        bool isActive = true,
        int inspectionCount = 5,
        int inspectionFormItemCount = 12)
    {
        // Create simple collections with the specified counts for testing
        var inspections = new List<Inspection>();
        for (int i = 0; i < inspectionCount; i++)
        {
            inspections.Add(null!); // Just for count - we only test the count property
        }

        var inspectionFormItems = new List<InspectionFormItem>();
        for (int i = 0; i < inspectionFormItemCount; i++)
        {
            inspectionFormItems.Add(null!); // Just for count - we only test the count property
        }

        return new InspectionForm
        {
            ID = id,
            Title = title,
            Description = description,
            IsActive = isActive,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inspections = inspections,
            InspectionFormItems = inspectionFormItems
        };
    }

    [Fact]
    public async Task Handler_Should_Return_InspectionFormDTO_When_Found()
    {
        // Arrange
        var query = CreateValidQuery();
        var inspectionForm = CreateInspectionFormEntity();
        _mockInspectionFormRepository.Setup(r => r.GetInspectionFormWithItemsAsync(query.InspectionFormID))
            .ReturnsAsync(inspectionForm);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(inspectionForm.ID, result.ID);
        Assert.Equal(inspectionForm.Title, result.Title);
        Assert.Equal(inspectionForm.Description, result.Description);
        Assert.Equal(inspectionForm.IsActive, result.IsActive);
        Assert.Equal(inspectionForm.CreatedAt, result.CreatedAt);
        Assert.Equal(inspectionForm.UpdatedAt, result.UpdatedAt);
        Assert.Equal(5, result.InspectionCount);
        Assert.Equal(12, result.InspectionFormItemCount);

        _mockInspectionFormRepository.Verify(r => r.GetInspectionFormWithItemsAsync(query.InspectionFormID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Not_Found()
    {
        // Arrange
        var query = CreateValidQuery();
        _mockInspectionFormRepository.Setup(r => r.GetInspectionFormWithItemsAsync(query.InspectionFormID))
            .ReturnsAsync((InspectionForm?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _queryHandler.Handle(query, CancellationToken.None));

        Assert.Equal(nameof(InspectionForm), exception.EntityName);
        Assert.Equal(nameof(InspectionForm.ID), exception.PropertyName);
        Assert.Equal(query.InspectionFormID.ToString(), exception.PropertyValue);

        _mockInspectionFormRepository.Verify(r => r.GetInspectionFormWithItemsAsync(query.InspectionFormID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Null_Collections_Gracefully()
    {
        // Arrange
        var query = CreateValidQuery();
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
        _mockInspectionFormRepository.Setup(r => r.GetInspectionFormWithItemsAsync(query.InspectionFormID))
            .ReturnsAsync(inspectionForm);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.InspectionCount);
        Assert.Equal(0, result.InspectionFormItemCount);
    }

    [Fact]
    public async Task Handler_Should_Handle_Empty_Collections()
    {
        // Arrange
        var query = CreateValidQuery();
        var inspectionForm = CreateInspectionFormEntity(inspectionCount: 0, inspectionFormItemCount: 0);
        _mockInspectionFormRepository.Setup(r => r.GetInspectionFormWithItemsAsync(query.InspectionFormID))
            .ReturnsAsync(inspectionForm);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.InspectionCount);
        Assert.Equal(0, result.InspectionFormItemCount);
    }

    [Fact]
    public async Task Handler_Should_Handle_Null_Description()
    {
        // Arrange
        var query = CreateValidQuery();
        var inspectionForm = CreateInspectionFormEntity(description: null);
        _mockInspectionFormRepository.Setup(r => r.GetInspectionFormWithItemsAsync(query.InspectionFormID))
            .ReturnsAsync(inspectionForm);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Description);
    }

    [Fact]
    public async Task Handler_Should_Handle_Inactive_InspectionForm()
    {
        // Arrange
        var query = CreateValidQuery();
        var inspectionForm = CreateInspectionFormEntity(isActive: false);
        _mockInspectionFormRepository.Setup(r => r.GetInspectionFormWithItemsAsync(query.InspectionFormID))
            .ReturnsAsync(inspectionForm);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsActive);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public async Task Handler_Should_Work_With_Different_IDs(int inspectionFormID)
    {
        // Arrange
        var query = CreateValidQuery(inspectionFormID);
        var inspectionForm = CreateInspectionFormEntity(id: inspectionFormID);
        _mockInspectionFormRepository.Setup(r => r.GetInspectionFormWithItemsAsync(inspectionFormID))
            .ReturnsAsync(inspectionForm);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(inspectionFormID, result.ID);
    }
}
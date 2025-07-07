using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Issues.Query.GetAllIssue;
using Application.MappingProfiles;
using Application.Models;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

using Xunit;

namespace Application.Test.Issues.QueryTest.GetAllIssue;

public class GetAllIssueQueryHandlerTest
{
    private readonly Mock<IIssueRepository> _mockIssueRepository;
    private readonly GetAllIssueQueryHandler _getAllIssueQueryHandler;
    private readonly Mock<IAppLogger<GetAllIssueQueryHandler>> _mockLogger;
    private readonly Mock<IValidator<GetAllIssueQuery>> _mockValidator;
    private readonly IMapper _mapper;

    public GetAllIssueQueryHandlerTest()
    {
        _mockIssueRepository = new();
        _mockLogger = new();
        _mockValidator = new();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<IssueMappingProfile>());
        _mapper = config.CreateMapper();
        _getAllIssueQueryHandler = new GetAllIssueQueryHandler(
            _mockIssueRepository.Object,
            _mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    private void SetupValidValidation(GetAllIssueQuery query)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.Validate(query)).Returns(validResult);
    }

    private void SetupInvalidValidation(GetAllIssueQuery query, string propertyName = "Parameters.PageNumber", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.Validate(query)).Returns(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_On_Success()
    {
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 5,
            Search = "Engine",
            SortBy = "title",
            SortDescending = false
        };
        var query = new GetAllIssueQuery(parameters);
        SetupValidValidation(query);

        var user = new User
        {
            Id = "USER1",
            FirstName = "Alice",
            LastName = "Smith",
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
        var vehicle = new Vehicle
        {
            ID = 10,
            Name = "Truck 1",
            Make = "Ford",
            Model = "F-150",
            Year = 2022,
            VIN = "VIN123",
            LicensePlate = "XYZ123",
            LicensePlateExpirationDate = DateTime.UtcNow.AddYears(1),
            VehicleType = VehicleTypeEnum.TRUCK,
            VehicleGroupID = 1,
            Trim = "XL",
            Mileage = 10000,
            EngineHours = 500,
            FuelCapacity = 80,
            FuelType = FuelTypeEnum.DIESEL,
            PurchaseDate = DateTime.UtcNow.AddYears(-1),
            PurchasePrice = 40000,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "Depot",
            AssignedTechnicianID = null,
            VehicleGroup = null!,
            User = user,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var expectedIssueEntities = new List<Issue>
        {
            new()
            {
                ID = 1,
                IssueNumber = 1001,
                VehicleID = 10,
                ReportedByUserID = "USER1",
                ReportedDate = DateTime.UtcNow.AddDays(-2),
                Title = "Engine Overheating",
                Description = "Engine temperature rises quickly.",
                Category = IssueCategoryEnum.ENGINE,
                PriorityLevel = PriorityLevelEnum.HIGH,
                Status = IssueStatusEnum.OPEN,
                ResolvedDate = null,
                ResolvedByUserID = null,
                ResolutionNotes = null,
                IssueAttachments = [],
                IssueAssignments = [],
                Vehicle = vehicle,
                User = user,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };
        _mockIssueRepository.Setup(r => r.GetAllIssuesPagedAsync(parameters)).ReturnsAsync(new PagedResult<Issue>
        {
            Items = expectedIssueEntities,
            TotalCount = expectedIssueEntities.Count,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        });

        var result = await _getAllIssueQueryHandler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.IsType<PagedResult<GetAllIssueDTO>>(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Single(result.Items);
        Assert.Equal("Engine Overheating", result.Items[0].Title);
        Assert.Equal(1001, result.Items[0].IssueNumber);
        Assert.Equal("USER1", result.Items[0].ReportedByUserID);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_Issues()
    {
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "NonExistentIssue"
        };
        var query = new GetAllIssueQuery(parameters);
        SetupValidValidation(query);
        _mockIssueRepository.Setup(r => r.GetAllIssuesPagedAsync(parameters)).ReturnsAsync(new PagedResult<Issue>
        {
            Items = new List<Issue>(),
            TotalCount = 0,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        });

        var result = await _getAllIssueQueryHandler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task Handler_Should_Handle_Different_Page_Sizes()
    {
        var allIssues = new List<Issue>();
        for (int i = 1; i <= 10; i++)
        {
            allIssues.Add(new Issue
            {
                ID = i,
                IssueNumber = 1000 + i,
                VehicleID = 10,
                ReportedByUserID = "USER1",
                ReportedDate = DateTime.UtcNow.AddDays(-i),
                Title = $"Issue {i}",
                Description = $"Description {i}",
                Category = IssueCategoryEnum.ENGINE,
                PriorityLevel = PriorityLevelEnum.HIGH,
                Status = IssueStatusEnum.OPEN,
                ResolvedDate = null,
                ResolvedByUserID = null,
                ResolutionNotes = null,
                IssueAttachments = [],
                IssueAssignments = [],
                Vehicle = null!,
                User = null!,
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                UpdatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }
        var parameters = new PaginationParameters
        {
            PageNumber = 2,
            PageSize = 3
        };
        var query = new GetAllIssueQuery(parameters);
        SetupValidValidation(query);
        _mockIssueRepository.Setup(r => r.GetAllIssuesPagedAsync(parameters)).ReturnsAsync(new PagedResult<Issue>
        {
            Items = allIssues.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList(),
            TotalCount = allIssues.Count,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        });

        var result = await _getAllIssueQueryHandler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(3, result.Items.Count); // Page 2, size 3
        Assert.Equal(4, result.Items[0].ID); // Should be the 4th issue
    }

    [Fact]
    public async Task Handler_Should_Handle_Last_Page()
    {
        var allIssues = new List<Issue>();
        for (int i = 1; i <= 12; i++)
        {
            allIssues.Add(new Issue
            {
                ID = i,
                IssueNumber = 1000 + i,
                VehicleID = 10,
                ReportedByUserID = "USER1",
                ReportedDate = DateTime.UtcNow.AddDays(-i),
                Title = $"Issue {i}",
                Description = $"Description {i}",
                Category = IssueCategoryEnum.ENGINE,
                PriorityLevel = PriorityLevelEnum.HIGH,
                Status = IssueStatusEnum.OPEN,
                ResolvedDate = null,
                ResolvedByUserID = null,
                ResolutionNotes = null,
                IssueAttachments = [],
                IssueAssignments = [],
                Vehicle = null!,
                User = null!,
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                UpdatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }
        var parameters = new PaginationParameters
        {
            PageNumber = 3,
            PageSize = 5
        };
        var query = new GetAllIssueQuery(parameters);
        SetupValidValidation(query);
        _mockIssueRepository.Setup(r => r.GetAllIssuesPagedAsync(parameters)).ReturnsAsync(new PagedResult<Issue>
        {
            Items = allIssues.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList(),
            TotalCount = allIssues.Count,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        });

        var result = await _getAllIssueQueryHandler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(3, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(2, result.Items.Count); // Only 2 items on last page
        Assert.Equal(11, result.Items[0].ID);
        Assert.Equal(12, result.Items[1].ID);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        var parameters = new PaginationParameters
        {
            PageNumber = 0,
            PageSize = 10
        };
        var query = new GetAllIssueQuery(parameters);
        SetupInvalidValidation(query, "Parameters.PageNumber", "Page number must be greater than 0");
        await Assert.ThrowsAsync<BadRequestException>(() => _getAllIssueQueryHandler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handler_Should_Search_By_Description()
    {
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10, Search = "overheating" };
        var query = new GetAllIssueQuery(parameters);
        SetupValidValidation(query);
        var issues = new List<Issue>
        {
            new Issue { ID = 1, Title = "Engine", Description = "Engine overheating", Category = IssueCategoryEnum.ENGINE, PriorityLevel = PriorityLevelEnum.HIGH, Status = IssueStatusEnum.OPEN, VehicleID = 1, IssueNumber = 1001, ReportedByUserID = "user1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! },
            new Issue { ID = 2, Title = "Brakes", Description = "Brake pads worn", Category = IssueCategoryEnum.BRAKES, PriorityLevel = PriorityLevelEnum.LOW, Status = IssueStatusEnum.RESOLVED, VehicleID = 2, IssueNumber = 1002, ReportedByUserID = "user2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! }
        };
        _mockIssueRepository.Setup(r => r.GetAllIssuesPagedAsync(parameters)).ReturnsAsync(new PagedResult<Issue> { Items = issues.Where(i => i.Description.ToLower().Contains("overheating")).ToList(), TotalCount = 1, PageNumber = 1, PageSize = 10 });
        var result = await _getAllIssueQueryHandler.Handle(query, CancellationToken.None);
        Assert.Single(result.Items);
        Assert.Equal("Engine overheating", result.Items[0].Description);
    }

    [Fact]
    public async Task Handler_Should_Search_By_Category()
    {
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10, Search = "brakes" };
        var query = new GetAllIssueQuery(parameters);
        SetupValidValidation(query);
        var issues = new List<Issue>
        {
            new Issue { ID = 1, Title = "Engine", Description = "Engine overheating", Category = IssueCategoryEnum.ENGINE, PriorityLevel = PriorityLevelEnum.HIGH, Status = IssueStatusEnum.OPEN, VehicleID = 1, IssueNumber = 1001, ReportedByUserID = "user1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! },
            new Issue { ID = 2, Title = "Brakes", Description = "Brake pads worn", Category = IssueCategoryEnum.BRAKES, PriorityLevel = PriorityLevelEnum.LOW, Status = IssueStatusEnum.RESOLVED, VehicleID = 2, IssueNumber = 1002, ReportedByUserID = "user2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! }
        };
        _mockIssueRepository.Setup(r => r.GetAllIssuesPagedAsync(parameters)).ReturnsAsync(new PagedResult<Issue> { Items = issues.Where(i => i.Category.ToString().ToLower().Contains("brakes")).ToList(), TotalCount = 1, PageNumber = 1, PageSize = 10 });
        var result = await _getAllIssueQueryHandler.Handle(query, CancellationToken.None);
        Assert.Single(result.Items);
        Assert.Equal(IssueCategoryEnum.BRAKES, result.Items[0].Category);
    }

    [Fact]
    public async Task Handler_Should_Search_By_PriorityLevel()
    {
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10, Search = "high" };
        var query = new GetAllIssueQuery(parameters);
        SetupValidValidation(query);
        var issues = new List<Issue>
        {
            new Issue { ID = 1, Title = "Engine", Description = "Engine overheating", Category = IssueCategoryEnum.ENGINE, PriorityLevel = PriorityLevelEnum.HIGH, Status = IssueStatusEnum.OPEN, VehicleID = 1, IssueNumber = 1001, ReportedByUserID = "user1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! },
            new Issue { ID = 2, Title = "Brakes", Description = "Brake pads worn", Category = IssueCategoryEnum.BRAKES, PriorityLevel = PriorityLevelEnum.LOW, Status = IssueStatusEnum.RESOLVED, VehicleID = 2, IssueNumber = 1002, ReportedByUserID = "user2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! }
        };
        _mockIssueRepository.Setup(r => r.GetAllIssuesPagedAsync(parameters)).ReturnsAsync(new PagedResult<Issue> { Items = issues.Where(i => i.PriorityLevel.ToString().ToLower().Contains("high")).ToList(), TotalCount = 1, PageNumber = 1, PageSize = 10 });
        var result = await _getAllIssueQueryHandler.Handle(query, CancellationToken.None);
        Assert.Single(result.Items);
        Assert.Equal(PriorityLevelEnum.HIGH, result.Items[0].PriorityLevel);
    }

    [Fact]
    public async Task Handler_Should_Search_By_Status()
    {
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10, Search = "resolved" };
        var query = new GetAllIssueQuery(parameters);
        SetupValidValidation(query);
        var issues = new List<Issue>
        {
            new Issue { ID = 1, Title = "Engine", Description = "Engine overheating", Category = IssueCategoryEnum.ENGINE, PriorityLevel = PriorityLevelEnum.HIGH, Status = IssueStatusEnum.OPEN, VehicleID = 1, IssueNumber = 1001, ReportedByUserID = "user1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! },
            new Issue { ID = 2, Title = "Brakes", Description = "Brake pads worn", Category = IssueCategoryEnum.BRAKES, PriorityLevel = PriorityLevelEnum.LOW, Status = IssueStatusEnum.RESOLVED, VehicleID = 2, IssueNumber = 1002, ReportedByUserID = "user2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! }
        };
        _mockIssueRepository.Setup(r => r.GetAllIssuesPagedAsync(parameters)).ReturnsAsync(new PagedResult<Issue> { Items = issues.Where(i => i.Status.ToString().ToLower().Contains("resolved")).ToList(), TotalCount = 1, PageNumber = 1, PageSize = 10 });
        var result = await _getAllIssueQueryHandler.Handle(query, CancellationToken.None);
        Assert.Single(result.Items);
        Assert.Equal(IssueStatusEnum.RESOLVED, result.Items[0].Status);
    }

    [Fact]
    public async Task Handler_Should_Search_By_VehicleID()
    {
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10, Search = "2" };
        var query = new GetAllIssueQuery(parameters);
        SetupValidValidation(query);
        var issues = new List<Issue>
        {
            new Issue { ID = 1, Title = "Engine", Description = "Engine overheating", Category = IssueCategoryEnum.ENGINE, PriorityLevel = PriorityLevelEnum.HIGH, Status = IssueStatusEnum.OPEN, VehicleID = 1, IssueNumber = 1001, ReportedByUserID = "user1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! },
            new Issue { ID = 2, Title = "Brakes", Description = "Brake pads worn", Category = IssueCategoryEnum.BRAKES, PriorityLevel = PriorityLevelEnum.LOW, Status = IssueStatusEnum.RESOLVED, VehicleID = 2, IssueNumber = 1002, ReportedByUserID = "user2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! }
        };
        _mockIssueRepository.Setup(r => r.GetAllIssuesPagedAsync(parameters)).ReturnsAsync(new PagedResult<Issue> { Items = issues.Where(i => i.VehicleID.ToString().Contains("2")).ToList(), TotalCount = 1, PageNumber = 1, PageSize = 10 });
        var result = await _getAllIssueQueryHandler.Handle(query, CancellationToken.None);
        Assert.Single(result.Items);
        Assert.Equal(2, result.Items[0].VehicleID);
    }

    [Fact]
    public async Task Handler_Should_Search_By_IssueNumber()
    {
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10, Search = "1002" };
        var query = new GetAllIssueQuery(parameters);
        SetupValidValidation(query);
        var issues = new List<Issue>
        {
            new Issue { ID = 1, Title = "Engine", Description = "Engine overheating", Category = IssueCategoryEnum.ENGINE, PriorityLevel = PriorityLevelEnum.HIGH, Status = IssueStatusEnum.OPEN, VehicleID = 1, IssueNumber = 1001, ReportedByUserID = "user1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! },
            new Issue { ID = 2, Title = "Brakes", Description = "Brake pads worn", Category = IssueCategoryEnum.BRAKES, PriorityLevel = PriorityLevelEnum.LOW, Status = IssueStatusEnum.RESOLVED, VehicleID = 2, IssueNumber = 1002, ReportedByUserID = "user2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! }
        };
        _mockIssueRepository.Setup(r => r.GetAllIssuesPagedAsync(parameters)).ReturnsAsync(new PagedResult<Issue> { Items = issues.Where(i => i.IssueNumber.ToString().Contains("1002")).ToList(), TotalCount = 1, PageNumber = 1, PageSize = 10 });
        var result = await _getAllIssueQueryHandler.Handle(query, CancellationToken.None);
        Assert.Single(result.Items);
        Assert.Equal(1002, result.Items[0].IssueNumber);
    }

    [Fact]
    public async Task Handler_Should_Search_By_ReportedByUserID()
    {
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10, Search = "user2" };
        var query = new GetAllIssueQuery(parameters);
        SetupValidValidation(query);
        var issues = new List<Issue>
        {
            new Issue { ID = 1, Title = "Engine", Description = "Engine overheating", Category = IssueCategoryEnum.ENGINE, PriorityLevel = PriorityLevelEnum.HIGH, Status = IssueStatusEnum.OPEN, VehicleID = 1, IssueNumber = 1001, ReportedByUserID = "user1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! },
            new Issue { ID = 2, Title = "Brakes", Description = "Brake pads worn", Category = IssueCategoryEnum.BRAKES, PriorityLevel = PriorityLevelEnum.LOW, Status = IssueStatusEnum.RESOLVED, VehicleID = 2, IssueNumber = 1002, ReportedByUserID = "user2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IssueAttachments = [], IssueAssignments = [], Vehicle = null!, User = null! }
        };
        _mockIssueRepository.Setup(r => r.GetAllIssuesPagedAsync(parameters)).ReturnsAsync(new PagedResult<Issue> { Items = issues.Where(i => i.ReportedByUserID.ToLower().Contains("user2")).ToList(), TotalCount = 1, PageNumber = 1, PageSize = 10 });
        var result = await _getAllIssueQueryHandler.Handle(query, CancellationToken.None);
        Assert.Single(result.Items);
        Assert.Equal("user2", result.Items[0].ReportedByUserID);
    }

    [Fact]
    public async Task Handler_Should_Return_ResolvedByUserName_When_ResolvedByUser_Is_Set()
    {
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 5,
            Search = "Engine",
            SortBy = "title",
            SortDescending = false
        };
        var query = new GetAllIssueQuery(parameters);
        SetupValidValidation(query);

        var reporter = new User
        {
            Id = "USER1",
            FirstName = "Alice",
            LastName = "Smith",
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
        var resolver = new User
        {
            Id = "USER2",
            FirstName = "Bob",
            LastName = "Jones",
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
        var vehicle = new Vehicle
        {
            ID = 10,
            Name = "Truck 1",
            Make = "Ford",
            Model = "F-150",
            Year = 2022,
            VIN = "VIN123",
            LicensePlate = "XYZ123",
            LicensePlateExpirationDate = DateTime.UtcNow.AddYears(1),
            VehicleType = VehicleTypeEnum.TRUCK,
            VehicleGroupID = 1,
            Trim = "XL",
            Mileage = 10000,
            EngineHours = 500,
            FuelCapacity = 80,
            FuelType = FuelTypeEnum.DIESEL,
            PurchaseDate = DateTime.UtcNow.AddYears(-1),
            PurchasePrice = 40000,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "Depot",
            AssignedTechnicianID = null,
            VehicleGroup = null!,
            User = reporter,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var expectedIssueEntities = new List<Issue>
        {
            new()
            {
                ID = 1,
                IssueNumber = 1001,
                VehicleID = 10,
                ReportedByUserID = "USER1",
                ReportedDate = DateTime.UtcNow.AddDays(-2),
                Title = "Engine Overheating",
                Description = "Engine temperature rises quickly.",
                Category = IssueCategoryEnum.ENGINE,
                PriorityLevel = PriorityLevelEnum.HIGH,
                Status = IssueStatusEnum.RESOLVED,
                ResolvedDate = DateTime.UtcNow.AddDays(-1),
                ResolvedByUserID = "USER2",
                ResolvedByUser = resolver,
                ResolutionNotes = "Replaced thermostat.",
                IssueAttachments = [],
                IssueAssignments = [],
                Vehicle = vehicle,
                User = reporter,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };
        _mockIssueRepository.Setup(r => r.GetAllIssuesPagedAsync(parameters)).ReturnsAsync(new PagedResult<Issue>
        {
            Items = expectedIssueEntities,
            TotalCount = expectedIssueEntities.Count,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        });

        var result = await _getAllIssueQueryHandler.Handle(query, CancellationToken.None);
        Assert.NotNull(result);
        Assert.IsType<PagedResult<GetAllIssueDTO>>(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("Engine Overheating", result.Items[0].Title);
        Assert.Equal(1001, result.Items[0].IssueNumber);
        Assert.Equal("USER1", result.Items[0].ReportedByUserID);
        Assert.Equal("Alice Smith", result.Items[0].ReportedByUserName);
        Assert.Equal("USER2", result.Items[0].ResolvedByUserID);
        Assert.Equal("Bob Jones", result.Items[0].ResolvedByUserName);
    }
}
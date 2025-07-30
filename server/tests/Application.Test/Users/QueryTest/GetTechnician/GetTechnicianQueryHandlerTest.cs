using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Users.Query.GetTechnician;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using Moq;

namespace Application.Test.Users.QueryTest.GetTechnician;

public class GetTechnicianQueryHandlerTest
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IAppLogger<GetTechnicianQueryHandler>> _mockLogger;
    private readonly GetTechnicianQueryHandler _handler;

    public GetTechnicianQueryHandlerTest()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<IAppLogger<GetTechnicianQueryHandler>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        var mapper = config.CreateMapper();

        _handler = new GetTechnicianQueryHandler(
            _mockUserRepository.Object,
            _mockLogger.Object,
            mapper
        );
    }

    [Fact]
    public async Task Handle_Should_Return_GetTechnicianDTO_On_Success()
    {
        // Given
        var query = new GetTechnicianQuery("guid-1");

        var expectedTechnician = new User
        {
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
        };

        _mockUserRepository.Setup(r => r.GetTechnicianByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedTechnician);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.Equal(expectedTechnician.Id, result.Id);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Technician_Not_Found()
    {
        // Given
        var query = new GetTechnicianQuery("non-existing-guid");

        _mockUserRepository.Setup(r => r.GetTechnicianByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(query, CancellationToken.None));

        _mockUserRepository.Verify(r => r.GetTechnicianByIdAsync(It.IsAny<string>()), Times.Once);
    }
}
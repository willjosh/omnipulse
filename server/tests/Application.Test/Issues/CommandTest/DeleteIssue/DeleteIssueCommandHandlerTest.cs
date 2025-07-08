
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Issues.Command.DeleteIssue;

using Domain.Entities;

using Moq;

using Xunit;

namespace Application.Test.Issues.CommandTest.DeleteIssue;

public class DeleteIssueCommandHandlerTest
{
    private readonly Mock<IIssueRepository> _mockIssueRepository;
    private readonly DeleteIssueCommandHandler _handler;
    private readonly Mock<IAppLogger<DeleteIssueCommandHandler>> _mockLogger;

    public DeleteIssueCommandHandlerTest()
    {
        _mockIssueRepository = new();
        _mockLogger = new();
        _handler = new DeleteIssueCommandHandler(_mockIssueRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_IssueID_On_Success()
    {
        // Given
        var command = new DeleteIssueCommand(IssueID: 42);
        var returnedIssue = new Issue
        {
            ID = command.IssueID,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Title = "Test Issue",
            Description = "Test Description",
            Category = Domain.Entities.Enums.IssueCategoryEnum.BODY,
            PriorityLevel = Domain.Entities.Enums.PriorityLevelEnum.CRITICAL,
            Status = Domain.Entities.Enums.IssueStatusEnum.IN_PROGRESS,
            VehicleID = 1,
            ReportedByUserID = "user1",
            IssueNumber = 1001,
            IssueAttachments = [],
            IssueAssignments = [],
            Vehicle = null!,
            ReportedByUser = null!
        };
        _mockIssueRepository.Setup(repo => repo.GetByIdAsync(command.IssueID)).ReturnsAsync(returnedIssue);
        _mockIssueRepository.Setup(repo => repo.Delete(returnedIssue));
        _mockIssueRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.IssueID, result);
        _mockIssueRepository.Verify(repo => repo.GetByIdAsync(command.IssueID), Times.Once);
        _mockIssueRepository.Verify(repo => repo.Delete(returnedIssue), Times.Once);
        _mockIssueRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_On_InvalidIssueID()
    {
        // Given
        var command = new DeleteIssueCommand(IssueID: 42);
        _mockIssueRepository.Setup(repo => repo.GetByIdAsync(command.IssueID)).ReturnsAsync((Issue?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        // Then
        _mockIssueRepository.Verify(repo => repo.GetByIdAsync(command.IssueID), Times.Once);
        _mockIssueRepository.Verify(repo => repo.Delete(It.IsAny<Issue>()), Times.Never);
        _mockIssueRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}
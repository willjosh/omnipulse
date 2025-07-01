using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.Issues.Command.CreateIssue;

public record CreateIssueCommand(
    int VehicleID,
    string ReportedByUserID,
    string Title,
    string? Description,
    IssueCategoryEnum Category,
    PriorityLevelEnum PriorityLevel,
    IssueStatusEnum Status
) : IRequest<int>;
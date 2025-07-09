using System;

using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.Issues.Command.UpdateIssue;

public record UpdateIssueCommand(
    int IssueID,
    int VehicleID,
    string Title,
    string? Description,
    PriorityLevelEnum PriorityLevel,
    IssueCategoryEnum Category,
    IssueStatusEnum Status,
    string ReportedByUserID,
    DateTime? ReportedDate,
    string? ResolutionNotes,
    DateTime? ResolvedDate,
    string? ResolvedByUserID
) : IRequest<int>;
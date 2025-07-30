import {
  IssueCategoryEnum,
  PriorityLevelEnum,
  IssueStatusEnum,
} from "@/features/issue/types/issueEnum";

export const getIssueCategoryLabel = (category: IssueCategoryEnum) => {
  switch (category) {
    case IssueCategoryEnum.ENGINE:
      return "Engine";
    case IssueCategoryEnum.TRANSMISSION:
      return "Transmission";
    case IssueCategoryEnum.BRAKES:
      return "Brakes";
    case IssueCategoryEnum.ELECTRICAL:
      return "Electrical";
    case IssueCategoryEnum.BODY:
      return "Body";
    case IssueCategoryEnum.TIRES:
      return "Tires";
    case IssueCategoryEnum.HVAC:
      return "HVAC";
    case IssueCategoryEnum.OTHER:
      return "Other";
    default:
      return "Unknown";
  }
};

export const getPriorityLevelLabel = (priority: PriorityLevelEnum) => {
  switch (priority) {
    case PriorityLevelEnum.LOW:
      return "Low";
    case PriorityLevelEnum.MEDIUM:
      return "Medium";
    case PriorityLevelEnum.HIGH:
      return "High";
    case PriorityLevelEnum.CRITICAL:
      return "Critical";
    default:
      return "Unknown";
  }
};

export const getIssueStatusLabel = (status: IssueStatusEnum) => {
  switch (status) {
    case IssueStatusEnum.OPEN:
      return "Open";
    case IssueStatusEnum.IN_PROGRESS:
      return "In Progress";
    case IssueStatusEnum.RESOLVED:
      return "Resolved";
    case IssueStatusEnum.CLOSED:
      return "Closed";
    case IssueStatusEnum.CANCELLED:
      return "Cancelled";
    default:
      return "Unknown";
  }
};

export function getTimeToResolve(
  reportedDate?: string | null,
  resolvedDate?: string | null,
): string {
  if (!reportedDate || !resolvedDate) return "Unknown";
  const start = new Date(reportedDate);
  const end = new Date(resolvedDate);
  const diffMs = end.getTime() - start.getTime();
  if (isNaN(diffMs) || diffMs < 0) return "Unknown";
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
  const diffHrs = Math.floor((diffMs / (1000 * 60 * 60)) % 24);
  const diffMin = Math.floor((diffMs / (1000 * 60)) % 60);
  let result = [];
  if (diffDays > 0)
    result.push(`${diffDays} ${diffDays === 1 ? "day" : "days"}`);
  if (diffHrs > 0) result.push(`${diffHrs} ${diffHrs === 1 ? "hr" : "hrs"}`);
  if (diffMin > 0) result.push(`${diffMin} ${diffMin === 1 ? "min" : "mins"}`);
  if (result.length === 0) result.push("0 min");
  return result.join(" ");
}

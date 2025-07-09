import {
  IssueCategoryEnum,
  PriorityLevelEnum,
  IssueStatusEnum,
} from "../_hooks/issue/issueEnum";

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

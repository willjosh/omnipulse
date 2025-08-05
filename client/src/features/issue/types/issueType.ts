import {
  IssueCategoryEnum,
  IssueStatusEnum,
  PriorityLevelEnum,
} from "./issueEnum";

export { IssueCategoryEnum, IssueStatusEnum, PriorityLevelEnum };

export interface Issue {
  id: number;
  issueNumber: number;
  vehicleID: number;
  vehicleName: string;
  title: string;
  description?: string | null;
  category: IssueCategoryEnum;
  priorityLevel: PriorityLevelEnum;
  status: IssueStatusEnum;
  reportedByUserID: string;
  reportedByUserName: string;
  reportedDate?: string | null;
  resolvedDate?: string | null;
  resolvedByUserID?: string | null;
  resolvedByUserName?: string | null;
  resolutionNotes?: string | null;
}

export interface IssueWithLabels
  extends Omit<Issue, "category" | "priorityLevel" | "status"> {
  category: number;
  categoryLabel: string;
  categoryEnum: IssueCategoryEnum;
  priorityLevel: number;
  priorityLevelLabel: string;
  priorityLevelEnum: PriorityLevelEnum;
  status: number;
  statusLabel: string;
  statusEnum: IssueStatusEnum;
}

export interface CreateIssueCommand {
  vehicleID: number;
  title: string;
  description?: string | null;
  priorityLevel: PriorityLevelEnum;
  category: IssueCategoryEnum;
  status: IssueStatusEnum;
  reportedByUserID: string;
  reportedDate?: string | null;
}

export interface UpdateIssueCommand {
  issueID: number;
  vehicleID: number;
  title: string;
  description?: string | null;
  priorityLevel: PriorityLevelEnum;
  category: IssueCategoryEnum;
  status: IssueStatusEnum;
  reportedByUserID: string;
  reportedDate?: string | null;
  resolutionNotes?: string | null;
  resolvedDate?: string | null;
  resolvedByUserID?: string | null;
}

export interface IssueFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}

export interface OpenIssueData {
  openIssueCount: number;
}

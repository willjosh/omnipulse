import {
  IssueCategoryEnum,
  IssueStatusEnum,
  PriorityLevelEnum,
} from "./issueEnum";

export { IssueCategoryEnum, IssueStatusEnum, PriorityLevelEnum };

export interface Issue {
  id: number;
  VehicleID: number;
  IssueNumber: number;
  ReportedByUserID: string;
  ReportedDate: string;
  Title: string;
  Description: string;
  Category: IssueCategoryEnum;
  PriorityLevel: PriorityLevelEnum;
  Status: IssueStatusEnum;
  ResolvedDate?: string | null;
  ResolvedByUserID?: string | null;
  ResolutionNotes?: string | null;
}

export interface IssueWithLabels
  extends Omit<Issue, "Category" | "PriorityLevel" | "Status"> {
  Category: number;
  CategoryLabel: string;
  CategoryEnum: IssueCategoryEnum;
  PriorityLevel: number;
  PriorityLevelLabel: string;
  PriorityLevelEnum: PriorityLevelEnum;
  Status: number;
  StatusLabel: string;
  StatusEnum: IssueStatusEnum;
}

export interface CreateIssueCommand {
  VehicleID: number;
  Title: string;
  Description?: string;
  PriorityLevel: PriorityLevelEnum;
  Category: IssueCategoryEnum;
  Status: IssueStatusEnum;
  ReportedByUserID: string;
  ReportedDate?: string;
}

export interface UpdateIssueCommand extends CreateIssueCommand {
  id: number;
}

export interface IssueFilter {
  page?: number;
  limit?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  search?: string;
  status?: number;
  priorityLevel?: number;
  category?: number;
  vehicleId?: number;
}

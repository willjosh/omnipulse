import {
  IssueCategoryEnum,
  IssueStatusEnum,
  PriorityLevelEnum,
} from "./issueEnum";

export { IssueCategoryEnum, IssueStatusEnum, PriorityLevelEnum };

export interface Issue {
  id: number;
  IssueNumber: number;
  VehicleID: number;
  VehicleName: string;
  Title: string;
  Description?: string | null;
  Category: IssueCategoryEnum;
  PriorityLevel: PriorityLevelEnum;
  Status: IssueStatusEnum;
  ReportedByUserID: string;
  ReportedByUserName: string;
  ReportedDate?: string | null;
  ResolvedDate?: string | null;
  ResolvedByUserID?: string | null;
  ResolvedByUserName?: string | null;
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
  Description?: string | null;
  PriorityLevel: PriorityLevelEnum;
  Category: IssueCategoryEnum;
  Status: IssueStatusEnum;
  ReportedByUserID: string;
  ReportedDate?: string | null;
}

export interface UpdateIssueCommand {
  id: number;
  VehicleID: number;
  Title: string;
  Description?: string | null;
  PriorityLevel: PriorityLevelEnum;
  Category: IssueCategoryEnum;
  Status: IssueStatusEnum;
  ReportedByUserID: string;
  ReportedDate?: string | null;
  ResolutionNotes?: string | null;
  ResolvedDate?: string | null;
  ResolvedByUserID?: string | null;
}

export interface IssueFilter {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  search?: string;
}

import { agent } from "@/lib/axios/agent";
import {
  Issue,
  IssueWithLabels,
  CreateIssueCommand,
  UpdateIssueCommand,
  IssueFilter,
  IssueCategoryEnum,
  PriorityLevelEnum,
  IssueStatusEnum,
  OpenIssueData,
} from "../types/issueType";
import {
  getIssueCategoryLabel,
  getPriorityLevelLabel,
  getIssueStatusLabel,
} from "../utils/issueEnumHelper";

function formatDate(date?: string | null): string {
  if (!date) return "Unknown";
  const d = new Date(date);
  return isNaN(d.getTime()) ? "Unknown" : d.toLocaleString();
}

export const convertIssueData = (issue: Issue): IssueWithLabels => ({
  ...issue,
  category: issue.category as number,
  categoryLabel: getIssueCategoryLabel(issue.category),
  categoryEnum: issue.category as IssueCategoryEnum,
  priorityLevel: issue.priorityLevel as number,
  priorityLevelLabel: getPriorityLevelLabel(issue.priorityLevel),
  priorityLevelEnum: issue.priorityLevel as PriorityLevelEnum,
  status: issue.status as number,
  statusLabel: getIssueStatusLabel(issue.status),
  statusEnum: issue.status as IssueStatusEnum,
  reportedDate: issue.reportedDate,
  resolvedDate: issue.resolvedDate,
});

export const convertIssueDataForDisplay = (issue: Issue): IssueWithLabels => ({
  ...issue,
  category: issue.category as number,
  categoryLabel: getIssueCategoryLabel(issue.category),
  categoryEnum: issue.category as IssueCategoryEnum,
  priorityLevel: issue.priorityLevel as number,
  priorityLevelLabel: getPriorityLevelLabel(issue.priorityLevel),
  priorityLevelEnum: issue.priorityLevel as PriorityLevelEnum,
  status: issue.status as number,
  statusLabel: getIssueStatusLabel(issue.status),
  statusEnum: issue.status as IssueStatusEnum,
  reportedDate: formatDate(issue.reportedDate),
  resolvedDate: formatDate(issue.resolvedDate),
});

export const issueApi = {
  getIssues: async (filter: IssueFilter) => {
    const queryParams = new URLSearchParams();
    if (filter.PageNumber)
      queryParams.append("PageNumber", filter.PageNumber.toString());
    if (filter.PageSize)
      queryParams.append("PageSize", filter.PageSize.toString());
    if (filter.Search) queryParams.append("Search", filter.Search);
    if (filter.SortBy) queryParams.append("SortBy", filter.SortBy);
    if (filter.SortDescending !== undefined)
      queryParams.append("SortDescending", filter.SortDescending.toString());
    const queryString = queryParams.toString();
    const { data } = await agent.get<{
      items: Issue[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/Issues${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  getIssue: async (id: number) => {
    const { data } = await agent.get<Issue>(`/api/Issues/${id}`);
    return data;
  },

  createIssue: async (command: CreateIssueCommand) => {
    const { data } = await agent.post("/api/Issues", command);
    return data;
  },

  updateIssue: async (command: UpdateIssueCommand) => {
    const { data } = await agent.put(`/api/Issues/${command.issueID}`, command);
    return data;
  },

  deleteIssue: async (id: number) => {
    const { data } = await agent.delete(`/api/Issues/${id}`);
    return data;
  },

  getOpenIssueData: async () => {
    const { data } = await agent.get<OpenIssueData>(
      "/api/Issues/OpenIssueData",
    );
    return data;
  },
};

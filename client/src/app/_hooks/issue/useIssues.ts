import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import {
  Issue,
  IssueWithLabels,
  CreateIssueCommand,
  UpdateIssueCommand,
  IssueFilter,
  IssueCategoryEnum,
  PriorityLevelEnum,
  IssueStatusEnum,
} from "./issueType";
import { useDebounce } from "../shared_types/useDebounce";
import { PagedResponse } from "@/app/_hooks/shared_types/pagedResponse";
import {
  getIssueCategoryLabel,
  getPriorityLevelLabel,
  getIssueStatusLabel,
} from "@/app/_utils/issueEnumHelper";

export const DEFAULT_PAGE_SIZE = 10;

function formatDate(date?: string | null): string {
  if (!date) return "Unknown";
  const d = new Date(date);
  return isNaN(d.getTime()) ? "Unknown" : d.toLocaleString();
}

// Helper to convert Issue to IssueWithLabels
const convertIssueData = (issue: Issue): IssueWithLabels => ({
  ...issue,
  Category: issue.Category as number,
  CategoryLabel: getIssueCategoryLabel(issue.Category),
  CategoryEnum: issue.Category as IssueCategoryEnum,
  PriorityLevel: issue.PriorityLevel as number,
  PriorityLevelLabel: getPriorityLevelLabel(issue.PriorityLevel),
  PriorityLevelEnum: issue.PriorityLevel as PriorityLevelEnum,
  Status: issue.Status as number,
  StatusLabel: getIssueStatusLabel(issue.Status),
  StatusEnum: issue.Status as IssueStatusEnum,
  ReportedDate: formatDate(issue.ReportedDate),
  ResolvedDate: formatDate(issue.ResolvedDate),
});

export function useIssues(filter: IssueFilter) {
  // Debounce the search parameter for consistency
  const debouncedSearch = useDebounce(filter?.search || "", 300);
  const debouncedFilter = { ...filter, search: debouncedSearch };

  // Build query params to match backend canonical names
  const queryParams = new URLSearchParams();
  if (debouncedFilter.page)
    queryParams.append("page", debouncedFilter.page.toString());
  if (debouncedFilter.pageSize)
    queryParams.append("pageSize", debouncedFilter.pageSize.toString());
  if (debouncedFilter.search)
    queryParams.append("search", debouncedFilter.search);
  if (debouncedFilter.sortBy)
    queryParams.append("sortBy", debouncedFilter.sortBy);
  if (debouncedFilter.sortOrder)
    queryParams.append("sortOrder", debouncedFilter.sortOrder);

  const queryString = queryParams.toString();

  const { data, isPending, isError, isSuccess, error } = useQuery<
    PagedResponse<IssueWithLabels>
  >({
    queryKey: ["issues", debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<Issue>>(
        `/issues${queryString ? `?${queryString}` : ""}`,
      );

      return { ...data, Items: data.Items.map(convertIssueData) };
    },
  });

  return {
    issues: data?.Items ?? [],
    pagination: data
      ? {
          totalCount: data.TotalCount,
          pageNumber: data.PageNumber,
          pageSize: data.PageSize,
          totalPages: data.TotalPages,
          hasPreviousPage: data.HasPreviousPage,
          hasNextPage: data.HasNextPage,
        }
      : null,
    isPending,
    isError,
    isSuccess,
    error,
  };
}

export function useIssue(id: number) {
  return useQuery({
    queryKey: ["issue", id],
    queryFn: async () => {
      const { data } = await agent.get<Issue>(`/issues/${id}`);
      return convertIssueData(data);
    },
    enabled: !!id,
  });
}

export function useCreateIssue() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: CreateIssueCommand) => {
      const { data } = await agent.post("/issues", command);
      return data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["issues"] });
    },
  });
}

export function useUpdateIssue() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: UpdateIssueCommand) => {
      const { data } = await agent.put(`/issues/${command.id}`, command);
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["issues"] });
      await queryClient.invalidateQueries({
        queryKey: ["issue", variables.id],
      });
    },
  });
}

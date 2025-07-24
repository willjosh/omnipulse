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

function formatDate(date?: string | null): string {
  if (!date) return "Unknown";
  const d = new Date(date);
  return isNaN(d.getTime()) ? "Unknown" : d.toLocaleString();
}

const convertIssueData = (issue: Issue): IssueWithLabels => ({
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

export function useIssues(filter: IssueFilter) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const queryParams = new URLSearchParams();
  if (debouncedFilter.PageNumber)
    queryParams.append("PageNumber", debouncedFilter.PageNumber.toString());
  if (debouncedFilter.PageSize)
    queryParams.append("PageSize", debouncedFilter.PageSize.toString());
  if (debouncedFilter.Search)
    queryParams.append("Search", debouncedFilter.Search);
  if (debouncedFilter.SortBy)
    queryParams.append("SortBy", debouncedFilter.SortBy);
  if (debouncedFilter.SortDescending !== undefined)
    queryParams.append(
      "SortDescending",
      debouncedFilter.SortDescending.toString(),
    );

  const queryString = queryParams.toString();

  const { data, isPending, isError, isSuccess, error } = useQuery<
    PagedResponse<IssueWithLabels>
  >({
    queryKey: ["issues", debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<Issue>>(
        `/api/Issues${queryString ? `?${queryString}` : ""}`,
      );
      return { ...data, items: data.items.map(convertIssueData) };
    },
  });

  return {
    issues: data?.items ?? [],
    pagination: data
      ? {
          totalCount: data.totalCount,
          pageNumber: data.pageNumber,
          pageSize: data.pageSize,
          totalPages: data.totalPages,
          hasPreviousPage: data.hasPreviousPage,
          hasNextPage: data.hasNextPage,
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
      const { data } = await agent.get<Issue>(`/api/Issues/${id}`);
      return convertIssueData(data);
    },
    enabled: !!id,
  });
}

export function useCreateIssue() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: CreateIssueCommand) => {
      const { data } = await agent.post("/api/Issues", command);
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
      const { data } = await agent.put(
        `/api/Issues/${command.issueID}`,
        command,
      );
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["issues"] });
      await queryClient.invalidateQueries({
        queryKey: ["issue", variables.issueID],
      });
    },
  });
}

export function useDeleteIssue() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: number) => {
      const { data } = await agent.delete(`/api/Issues/${id}`);
      return data;
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["issues"] });
      await queryClient.invalidateQueries({ queryKey: ["issue", id] });
    },
  });
}

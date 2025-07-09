import { useQuery, useMutation } from "@tanstack/react-query";
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
});

// Fetch paged issues
const fetchIssues = async (
  params: IssueFilter,
): Promise<PagedResponse<IssueWithLabels>> => {
  const { data } = await agent.get("/issues", { params });
  return { ...data, Items: data.Items.map(convertIssueData) };
};

// Fetch single issue
const fetchIssue = async (id: number): Promise<IssueWithLabels> => {
  const { data } = await agent.get(`/issues/${id}`);
  return convertIssueData(data);
};

// Create issue
const createIssue = async (command: CreateIssueCommand): Promise<Issue> => {
  const { data } = await agent.post("/issues", command);
  return data;
};

// Update issue
const updateIssue = async (command: UpdateIssueCommand): Promise<Issue> => {
  const { data } = await agent.put(`/issues/${command.id}`, command);
  return data;
};

// Deactivate (archive) issue
const deactivateIssue = async (id: number): Promise<void> => {
  await agent.patch(`/issues/${id}/deactivate`);
};

export function useIssues(filter: IssueFilter) {
  // Debounce the search parameter for consistency
  const debouncedSearch = useDebounce(filter?.search || "", 300);
  const debouncedFilter = { ...filter, search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["issues", debouncedFilter],
    queryFn: () => fetchIssues(debouncedFilter),
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
    queryFn: () => fetchIssue(id),
    enabled: !!id,
  });
}

export function useCreateIssue() {
  return useMutation({ mutationFn: createIssue });
}

export function useUpdateIssue() {
  return useMutation({ mutationFn: updateIssue });
}

export function useDeactivateIssue() {
  return useMutation({ mutationFn: deactivateIssue });
}

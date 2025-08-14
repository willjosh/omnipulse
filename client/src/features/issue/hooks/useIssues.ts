import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  issueApi,
  convertIssueData,
  convertIssueDataForDisplay,
} from "../api/issueApi";
import {
  IssueWithLabels,
  IssueFilter,
  OpenIssueData,
} from "../types/issueType";
import { useDebounce } from "@/hooks/useDebounce";

export function useIssues(filter: IssueFilter) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["issues", debouncedFilter],
    queryFn: async () => {
      const data = await issueApi.getIssues(debouncedFilter);
      return { ...data, items: data.items.map(convertIssueDataForDisplay) };
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
  const { data, isPending, isError, isSuccess, error } =
    useQuery<IssueWithLabels>({
      queryKey: ["issue", id],
      queryFn: async () => {
        const data = await issueApi.getIssue(id);
        return convertIssueData(data);
      },
      enabled: !!id,
    });

  return { issue: data, isPending, isError, isSuccess, error };
}

export function useOpenIssueData() {
  const { data, isPending, isError, isSuccess, error } =
    useQuery<OpenIssueData>({
      queryKey: ["openIssueData"],
      queryFn: async () => {
        const data = await issueApi.getOpenIssueData();
        return data;
      },
    });

  return {
    openIssueCount: data?.openIssueCount ?? 0,
    isLoadingOpenIssueData: isPending,
    isError,
    isSuccess,
    error,
  };
}

export function useCreateIssue() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: issueApi.createIssue,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["issues"] });
    },
  });
}

export function useUpdateIssue() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: issueApi.updateIssue,
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
    mutationFn: issueApi.deleteIssue,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["issues"] });
      await queryClient.invalidateQueries({ queryKey: ["issue", id] });
    },
  });
}

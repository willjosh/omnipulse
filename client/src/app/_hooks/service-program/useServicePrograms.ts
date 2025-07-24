import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import {
  ServiceProgram,
  CreateServiceProgramCommand,
  UpdateServiceProgramCommand,
  ServiceProgramFilter,
} from "./serviceProgramType";
import { PagedResponse } from "@/app/_hooks/shared_types/pagedResponse";
import { useDebounce } from "@/app/_hooks/shared_types/useDebounce";

export function useServicePrograms(filter: ServiceProgramFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const queryParams = new URLSearchParams();
  if (debouncedFilter.PageNumber)
    queryParams.append("PageNumber", debouncedFilter.PageNumber.toString());
  if (debouncedFilter.PageSize)
    queryParams.append("PageSize", debouncedFilter.PageSize.toString());
  if (debouncedFilter.SortBy)
    queryParams.append("SortBy", debouncedFilter.SortBy);
  if (debouncedFilter.SortDescending !== undefined)
    queryParams.append(
      "SortDescending",
      debouncedFilter.SortDescending.toString(),
    );
  if (debouncedFilter.Search)
    queryParams.append("Search", debouncedFilter.Search);

  const queryString = queryParams.toString();

  const { data, isPending, isError, isSuccess, error } = useQuery<
    PagedResponse<ServiceProgram>
  >({
    queryKey: ["servicePrograms", debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<ServiceProgram>>(
        `/api/ServicePrograms${queryString ? `?${queryString}` : ""}`,
      );
      return data;
    },
  });

  return {
    servicePrograms: data?.items ?? [],
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

export function useServiceProgram(id: number) {
  return useQuery<ServiceProgram>({
    queryKey: ["serviceProgram", id],
    queryFn: async () => {
      const { data } = await agent.get<ServiceProgram>(
        `/api/ServicePrograms/${id}`,
      );
      return data;
    },
    enabled: !!id,
  });
}

export function useCreateServiceProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: CreateServiceProgramCommand) => {
      const { data } = await agent.post("/api/ServicePrograms", command);
      return data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["servicePrograms"] });
    },
  });
}

export function useUpdateServiceProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: UpdateServiceProgramCommand) => {
      const { data } = await agent.put(
        `/api/ServicePrograms/${command.serviceProgramID}`,
        command,
      );
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["servicePrograms"] });
      await queryClient.invalidateQueries({
        queryKey: ["serviceProgram", variables.serviceProgramID],
      });
    },
  });
}

export function useDeleteServiceProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: number) => {
      const { data } = await agent.delete(`/api/ServicePrograms/${id}`);
      return data;
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["servicePrograms"] });
      await queryClient.invalidateQueries({ queryKey: ["serviceProgram", id] });
    },
  });
}

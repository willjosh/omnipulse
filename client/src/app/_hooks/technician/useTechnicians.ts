import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import {
  Technician,
  TechnicianFilter,
  CreateTechnicianCommand,
  UpdateTechnicianCommand,
} from "./technicianType";
import { PagedResponse } from "@/app/_hooks/shared_types/pagedResponse";
import { useDebounce } from "@/app/_hooks/shared_types/useDebounce";

export function useTechnicians(filter: TechnicianFilter = {}) {
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
    PagedResponse<Technician>
  >({
    queryKey: ["technicians", debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<Technician>>(
        `/api/Technicians${queryString ? `?${queryString}` : ""}`,
      );
      return data;
    },
  });

  return {
    technicians: data?.items ?? [],
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

export function useTechnician(id: string) {
  return useQuery({
    queryKey: ["technician", id],
    queryFn: async () => {
      const { data } = await agent.get<Technician>(`/api/Technicians/${id}`);
      return data;
    },
    enabled: !!id,
  });
}

export function useCreateTechnician() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: CreateTechnicianCommand) => {
      const { data } = await agent.post("/api/Technicians", command);
      return data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["technicians"] });
    },
  });
}

export function useUpdateTechnician() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: UpdateTechnicianCommand) => {
      const { data } = await agent.put(
        `/api/Technicians/${command.id}`,
        command,
      );
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["technicians"] });
      await queryClient.invalidateQueries({
        queryKey: ["technician", variables.id],
      });
    },
  });
}

export function useDeactivateTechnician() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => {
      const { data } = await agent.patch(`/api/Technicians/${id}/deactivate`);
      return data;
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["technicians"] });
      await queryClient.invalidateQueries({ queryKey: ["technician", id] });
    },
  });
}

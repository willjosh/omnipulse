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
  const debouncedSearch = useDebounce(filter?.search || "", 300);
  const debouncedFilter = { ...filter, search: debouncedSearch };

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
    PagedResponse<Technician>
  >({
    queryKey: ["technicians", debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<Technician>>(
        `/technicians${queryString ? `?${queryString}` : ""}`,
      );
      return data;
    },
  });

  return {
    technicians: data?.Items ?? [],
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

export function useTechnician(id: string) {
  return useQuery({
    queryKey: ["technician", id],
    queryFn: async () => {
      const { data } = await agent.get(`/technicians/${id}`);
      return data as Technician;
    },
    enabled: !!id,
  });
}

export function useCreateTechnician() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: CreateTechnicianCommand) => {
      const { data } = await agent.post("/technicians", command);
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
      const { data } = await agent.put(`/technicians/${command.id}`, command);
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

export function useHandleTechnicianStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => {
      const { data } = await agent.post(`/technicians/status/${id}`);
      return data;
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["technicians"] });
      await queryClient.invalidateQueries({ queryKey: ["technician", id] });
    },
  });
}

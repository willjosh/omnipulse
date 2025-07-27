import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import {
  ServiceProgramVehicle,
  AddVehicleToServiceProgramCommand,
  RemoveVehicleFromServiceProgramCommand,
  RemoveVehicleFromServiceProgramResponse,
  ServiceProgramVehicleFilter,
} from "./serviceProgramVehicleType";
import { PagedResponse } from "@/app/_hooks/shared_types/pagedResponse";
import { useDebounce } from "@/app/_hooks/shared_types/useDebounce";

export function useServiceProgramVehicles(
  serviceProgramId: number,
  filter: ServiceProgramVehicleFilter = {},
) {
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
    PagedResponse<ServiceProgramVehicle>
  >({
    queryKey: ["serviceProgramVehicles", serviceProgramId, debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<ServiceProgramVehicle>>(
        `/api/ServicePrograms/${serviceProgramId}/vehicles${queryString ? `?${queryString}` : ""}`,
      );
      return data;
    },
    enabled: !!serviceProgramId,
  });

  return {
    serviceProgramVehicles: data?.items ?? [],
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

export function useAddVehicleToServiceProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: AddVehicleToServiceProgramCommand) => {
      const { data } = await agent.post(
        `/api/ServicePrograms/${command.serviceProgramID}/vehicles`,
        command,
      );
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({
        queryKey: ["serviceProgramVehicles", variables.serviceProgramID],
      });
      await queryClient.invalidateQueries({
        queryKey: ["serviceProgram", variables.serviceProgramID],
      });
    },
  });
}

export function useRemoveVehicleFromServiceProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: RemoveVehicleFromServiceProgramCommand) => {
      const { data } =
        await agent.delete<RemoveVehicleFromServiceProgramResponse>(
          `/api/ServicePrograms/${command.serviceProgramID}/vehicles/${command.vehicleID}`,
        );
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({
        queryKey: ["serviceProgramVehicles", variables.serviceProgramID],
      });
      await queryClient.invalidateQueries({
        queryKey: ["serviceProgram", variables.serviceProgramID],
      });
    },
  });
}

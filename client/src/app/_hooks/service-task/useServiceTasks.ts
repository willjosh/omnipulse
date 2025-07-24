import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import {
  ServiceTask,
  ServiceTaskWithLabels,
  ServiceTaskFilter,
  CreateServiceTaskCommand,
  UpdateServiceTaskCommand,
} from "./serviceTaskType";
import { ServiceTaskCategoryEnum } from "./serviceTaskEnum";
import { PagedResponse } from "@/app/_hooks/shared_types/pagedResponse";
import { useDebounce } from "@/app/_hooks/shared_types/useDebounce";
import { getServiceTaskCategoryLabel } from "@/app/_utils/serviceTaskEnumHelper";

export const convertServiceTaskData = (
  task: ServiceTask,
): ServiceTaskWithLabels => ({
  ...task,
  category: task.category as number,
  categoryLabel: getServiceTaskCategoryLabel(task.category),
  categoryEnum: task.category as ServiceTaskCategoryEnum,
});

export function useServiceTasks(filter: ServiceTaskFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, search: debouncedSearch };

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
    PagedResponse<ServiceTaskWithLabels>
  >({
    queryKey: ["serviceTasks", debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<ServiceTask>>(
        `/api/ServiceTasks${queryString ? `?${queryString}` : ""}`,
      );
      return { ...data, items: data.items.map(convertServiceTaskData) };
    },
  });

  return {
    serviceTasks: data?.items ?? [],
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

export function useServiceTask(id: number) {
  return useQuery<ServiceTaskWithLabels>({
    queryKey: ["serviceTask", id],
    queryFn: async () => {
      const { data } = await agent.get<ServiceTask>(`/api/ServiceTasks/${id}`);
      return convertServiceTaskData(data);
    },
    enabled: !!id,
  });
}

export function useCreateServiceTask() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: CreateServiceTaskCommand) => {
      const { data } = await agent.post("/api/ServiceTasks", command);
      return data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["serviceTasks"] });
    },
  });
}

export function useUpdateServiceTask() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: UpdateServiceTaskCommand) => {
      const { data } = await agent.put(
        `/api/ServiceTasks/${command.ServiceTaskID}`,
        command,
      );
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["serviceTasks"] });
      await queryClient.invalidateQueries({
        queryKey: ["serviceTask", variables.ServiceTaskID],
      });
    },
  });
}

export function useDeleteServiceTask() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: number) => {
      const { data } = await agent.delete(`/api/ServiceTasks/${id}`);
      return data;
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["serviceTasks"] });
      await queryClient.invalidateQueries({ queryKey: ["serviceTask", id] });
    },
  });
}

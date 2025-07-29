import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { serviceTaskApi } from "../api/serviceTaskApi";
import {
  ServiceTask,
  ServiceTaskWithLabels,
  ServiceTaskFilter,
} from "../types/serviceTaskType";
import { ServiceTaskCategoryEnum } from "../types/serviceTaskEnum";
import { useDebounce } from "@/hooks/useDebounce";
import { getServiceTaskCategoryLabel } from "@/features/service-task/utils/serviceTaskEnumHelper";

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

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["serviceTasks", debouncedFilter],
    queryFn: async () => {
      const data = await serviceTaskApi.getServiceTasks(debouncedFilter);
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
  const { data, isPending, isError, isSuccess, error } =
    useQuery<ServiceTaskWithLabels>({
      queryKey: ["serviceTask", id],
      queryFn: async () => {
        const data = await serviceTaskApi.getServiceTask(id);
        return convertServiceTaskData(data);
      },
      enabled: !!id,
    });

  return { serviceTask: data, isPending, isError, isSuccess, error };
}

export function useCreateServiceTask() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: serviceTaskApi.createServiceTask,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["serviceTasks"] });
    },
  });
}

export function useUpdateServiceTask() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: serviceTaskApi.updateServiceTask,
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
    mutationFn: serviceTaskApi.deleteServiceTask,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["serviceTasks"] });
      await queryClient.invalidateQueries({ queryKey: ["serviceTask", id] });
    },
  });
}

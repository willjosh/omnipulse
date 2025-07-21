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
  Category: task.Category as number,
  CategoryLabel: getServiceTaskCategoryLabel(task.Category),
  CategoryEnum: task.Category as ServiceTaskCategoryEnum,
});

export function useServiceTasks(filter: ServiceTaskFilter = {}) {
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
    PagedResponse<ServiceTaskWithLabels>
  >({
    queryKey: ["serviceTasks", debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<ServiceTask>>(
        `/serviceTasks${queryString ? `?${queryString}` : ""}`,
      );
      return { ...data, Items: data.Items.map(convertServiceTaskData) };
    },
  });

  return {
    serviceTasks: data?.Items ?? [],
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

export function useServiceTask(id: number) {
  return useQuery<ServiceTaskWithLabels>({
    queryKey: ["serviceTask", id],
    queryFn: async () => {
      const { data } = await agent.get<ServiceTask>(`/serviceTasks/${id}`);
      return convertServiceTaskData(data);
    },
    enabled: !!id,
  });
}

export function useCreateServiceTask() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: CreateServiceTaskCommand) => {
      const { data } = await agent.post("/serviceTasks", command);
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
      const { data } = await agent.put(`/serviceTasks/${command.id}`, command);
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["serviceTasks"] });
      await queryClient.invalidateQueries({
        queryKey: ["serviceTask", variables.id],
      });
    },
  });
}

export function useDeactivateServiceTask() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: number) => {
      const { data } = await agent.post(`/serviceTasks/deactivate/${id}`);
      return data;
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["serviceTasks"] });
      await queryClient.invalidateQueries({ queryKey: ["serviceTask", id] });
    },
  });
}

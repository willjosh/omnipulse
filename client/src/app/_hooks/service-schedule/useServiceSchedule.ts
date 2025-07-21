import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import {
  ServiceSchedule,
  ServiceScheduleWithLabels,
  ServiceScheduleFilter,
  CreateServiceScheduleCommand,
  UpdateServiceScheduleCommand,
} from "./serviceScheduleType";
import { TimeUnitEnum } from "./serviceScheduleEnum";
import { PagedResponse } from "@/app/_hooks/shared_types/pagedResponse";
import { useDebounce } from "@/app/_hooks/shared_types/useDebounce";
import { getTimeUnitEnumLabel } from "@/app/_utils/serviceScheduleEnumHelper";
import { convertServiceTaskData } from "../service-task/useServiceTask";

const convertServiceScheduleData = (
  schedule: ServiceSchedule,
): ServiceScheduleWithLabels => ({
  ...schedule,
  ServiceTasks: schedule.ServiceTasks.map(convertServiceTaskData),
  TimeIntervalUnit: schedule.TimeIntervalUnit as number,
  TimeIntervalUnitLabel: getTimeUnitEnumLabel(schedule.TimeIntervalUnit),
  TimeIntervalUnitEnum: schedule.TimeIntervalUnit as TimeUnitEnum,
  TimeBufferUnit: schedule.TimeBufferUnit as number,
  TimeBufferUnitLabel: getTimeUnitEnumLabel(schedule.TimeBufferUnit),
  TimeBufferUnitEnum: schedule.TimeBufferUnit as TimeUnitEnum,
  FirstServiceTimeUnit: schedule.FirstServiceTimeUnit as number,
  FirstServiceTimeUnitLabel: getTimeUnitEnumLabel(
    schedule.FirstServiceTimeUnit,
  ),
  FirstServiceTimeUnitEnum: schedule.FirstServiceTimeUnit as TimeUnitEnum,
});

export function useServiceSchedules(filter: ServiceScheduleFilter = {}) {
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
    PagedResponse<ServiceScheduleWithLabels>
  >({
    queryKey: ["serviceSchedules", debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<ServiceSchedule>>(
        `/serviceSchedules${queryString ? `?${queryString}` : ""}`,
      );
      return { ...data, Items: data.Items.map(convertServiceScheduleData) };
    },
  });

  return {
    serviceSchedules: data?.Items ?? [],
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

export function useServiceSchedule(id: number) {
  return useQuery<ServiceScheduleWithLabels>({
    queryKey: ["serviceSchedule", id],
    queryFn: async () => {
      const { data } = await agent.get<ServiceSchedule>(
        `/serviceSchedules/${id}`,
      );
      return convertServiceScheduleData(data);
    },
    enabled: !!id,
  });
}

export function useCreateServiceSchedule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: CreateServiceScheduleCommand) => {
      const { data } = await agent.post("/serviceSchedules", command);
      return data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["serviceSchedules"] });
    },
  });
}

export function useUpdateServiceSchedule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (command: UpdateServiceScheduleCommand) => {
      const { data } = await agent.put(
        `/serviceSchedules/${command.ServiceScheduleID}`,
        command,
      );
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["serviceSchedules"] });
      await queryClient.invalidateQueries({
        queryKey: ["serviceSchedule", variables.ServiceScheduleID],
      });
    },
  });
}

export function useDeleteServiceSchedule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: number) => {
      await agent.delete(`/serviceSchedules/${id}`);
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["serviceSchedules"] });
      await queryClient.invalidateQueries({
        queryKey: ["serviceSchedule", id],
      });
    },
  });
}

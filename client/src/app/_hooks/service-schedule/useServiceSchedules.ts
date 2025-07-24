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
import { convertServiceTaskData } from "../service-task/useServiceTasks";

const convertServiceScheduleData = (
  schedule: ServiceSchedule,
): ServiceScheduleWithLabels => ({
  ...schedule,
  serviceTasks: schedule.serviceTasks.map(convertServiceTaskData),
  timeIntervalUnit: schedule.timeIntervalUnit as number,
  timeIntervalUnitLabel: getTimeUnitEnumLabel(schedule.timeIntervalUnit),
  timeIntervalUnitEnum: schedule.timeIntervalUnit as TimeUnitEnum,
  timeBufferUnit: schedule.timeBufferUnit as number,
  timeBufferUnitLabel: getTimeUnitEnumLabel(schedule.timeBufferUnit),
  timeBufferUnitEnum: schedule.timeBufferUnit as TimeUnitEnum,
  firstServiceTimeUnit: schedule.firstServiceTimeUnit as number,
  firstServiceTimeUnitLabel: getTimeUnitEnumLabel(
    schedule.firstServiceTimeUnit,
  ),
  firstServiceTimeUnitEnum: schedule.firstServiceTimeUnit as TimeUnitEnum,
});

export function useServiceSchedules(filter: ServiceScheduleFilter = {}) {
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
    PagedResponse<ServiceScheduleWithLabels>
  >({
    queryKey: ["serviceSchedules", debouncedFilter],
    queryFn: async () => {
      const { data } = await agent.get<PagedResponse<ServiceSchedule>>(
        `/api/ServiceSchedules${queryString ? `?${queryString}` : ""}`,
      );
      return { ...data, items: data.items.map(convertServiceScheduleData) };
    },
  });

  return {
    serviceSchedules: data?.items ?? [],
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

export function useServiceSchedule(id: number) {
  return useQuery<ServiceScheduleWithLabels>({
    queryKey: ["serviceSchedule", id],
    queryFn: async () => {
      const { data } = await agent.get<ServiceSchedule>(
        `/api/ServiceSchedules/${id}`,
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
      const { data } = await agent.post("/api/ServiceSchedules", command);
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
        `/api/ServiceSchedules/${command.serviceScheduleID}`,
        command,
      );
      return data;
    },
    onSuccess: async (_data, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["serviceSchedules"] });
      await queryClient.invalidateQueries({
        queryKey: ["serviceSchedule", variables.serviceScheduleID],
      });
    },
  });
}

export function useDeleteServiceSchedule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: number) => {
      const { data } = await agent.delete(`/api/ServiceSchedules/${id}`);
      return data;
    },
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["serviceSchedules"] });
      await queryClient.invalidateQueries({
        queryKey: ["serviceSchedule", id],
      });
    },
  });
}

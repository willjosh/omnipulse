import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { serviceScheduleApi } from "../api/serviceScheduleApi";
import {
  ServiceSchedule,
  ServiceScheduleWithLabels,
  ServiceScheduleFilter,
  CreateServiceScheduleCommand,
  UpdateServiceScheduleCommand,
} from "../types/serviceScheduleType";
import { TimeUnitEnum } from "../types/serviceScheduleEnum";
import { useDebounce } from "@/hooks/useDebounce";
import { getTimeUnitEnumLabel } from "@/features/service-schedule/utils/serviceScheduleEnumHelper";
import { convertServiceTaskData } from "../../service-task/hooks/useServiceTasks";

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

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["serviceSchedules", debouncedFilter],
    queryFn: async () => {
      const data =
        await serviceScheduleApi.getServiceSchedules(debouncedFilter);
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
  const { data, isPending, isError, isSuccess, error } =
    useQuery<ServiceScheduleWithLabels>({
      queryKey: ["serviceSchedule", id],
      queryFn: async () => {
        const data = await serviceScheduleApi.getServiceSchedule(id);
        return convertServiceScheduleData(data);
      },
      enabled: !!id,
    });

  return { serviceSchedule: data, isPending, isError, isSuccess, error };
}

export function useCreateServiceSchedule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: serviceScheduleApi.createServiceSchedule,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["serviceSchedules"] });
    },
  });
}

export function useUpdateServiceSchedule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: serviceScheduleApi.updateServiceSchedule,
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
    mutationFn: serviceScheduleApi.deleteServiceSchedule,
    onSuccess: async (_data, id) => {
      await queryClient.invalidateQueries({ queryKey: ["serviceSchedules"] });
      await queryClient.invalidateQueries({
        queryKey: ["serviceSchedule", id],
      });
    },
  });
}

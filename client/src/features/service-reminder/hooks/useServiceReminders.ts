import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  serviceReminderApi,
  convertServiceReminderData,
} from "../api/serviceReminderApi";
import {
  ServiceReminderWithLabels,
  ServiceReminderFilter,
} from "../types/serviceReminderType";
import { useDebounce } from "@/hooks/useDebounce";

export function useServiceReminders(filter: ServiceReminderFilter = {}) {
  const debouncedSearch = useDebounce(filter?.Search || "", 300);
  const debouncedFilter = { ...filter, Search: debouncedSearch };

  const { data, isPending, isError, isSuccess, error } = useQuery({
    queryKey: ["serviceReminders", debouncedFilter],
    queryFn: async () => {
      const data =
        await serviceReminderApi.getServiceReminders(debouncedFilter);
      return { ...data, items: data.items.map(convertServiceReminderData) };
    },
  });

  return {
    serviceReminders: data?.items ?? [],
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

export function useAddWorkOrderToServiceReminder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      serviceReminderId,
      workOrderId,
    }: {
      serviceReminderId: number;
      workOrderId: number;
    }) =>
      serviceReminderApi.addWorkOrderToServiceReminder(
        serviceReminderId,
        workOrderId,
      ),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["serviceReminders"] });
    },
  });
}

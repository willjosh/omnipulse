import { agent } from "@/lib/axios/agent";
import {
  ServiceReminder,
  ServiceReminderWithLabels,
  ServiceReminderFilter,
} from "../types/serviceReminderType";
import {
  ServiceReminderStatusEnum,
  PriorityLevelEnum,
  TimeUnitEnum,
  ServiceTaskCategoryEnum,
  ServiceScheduleTypeEnum,
} from "../types/serviceReminderEnum";
import {
  getServiceReminderStatusLabel,
  getPriorityLevelLabel,
  getTimeUnitLabel,
  getServiceTaskCategoryLabel,
  getServiceScheduleTypeLabel,
} from "../utils/serviceReminderEnumHelper";

export const convertServiceReminderData = (
  serviceReminder: ServiceReminder,
): ServiceReminderWithLabels => ({
  ...serviceReminder,
  serviceTasks: serviceReminder.serviceTasks.map(task => ({
    ...task,
    serviceTaskCategory: task.serviceTaskCategory as number,
    serviceTaskCategoryLabel: getServiceTaskCategoryLabel(
      task.serviceTaskCategory,
    ),
    serviceTaskCategoryEnum:
      task.serviceTaskCategory as ServiceTaskCategoryEnum,
  })),
  status: serviceReminder.status as number,
  statusLabel: getServiceReminderStatusLabel(serviceReminder.status),
  statusEnum: serviceReminder.status as ServiceReminderStatusEnum,
  priorityLevel: serviceReminder.priorityLevel as number,
  priorityLevelLabel: getPriorityLevelLabel(serviceReminder.priorityLevel),
  priorityLevelEnum: serviceReminder.priorityLevel as PriorityLevelEnum,
  timeIntervalUnit: serviceReminder.timeIntervalUnit as number,
  timeIntervalUnitLabel: getTimeUnitLabel(serviceReminder.timeIntervalUnit),
  timeIntervalUnitEnum: serviceReminder.timeIntervalUnit as TimeUnitEnum,
  timeBufferUnit: serviceReminder.timeBufferUnit as number,
  timeBufferUnitLabel: getTimeUnitLabel(serviceReminder.timeBufferUnit),
  timeBufferUnitEnum: serviceReminder.timeBufferUnit as TimeUnitEnum,
  scheduleType: serviceReminder.scheduleType as number,
  scheduleTypeLabel: getServiceScheduleTypeLabel(serviceReminder.scheduleType),
  scheduleTypeEnum: serviceReminder.scheduleType as ServiceScheduleTypeEnum,
});

export const serviceReminderApi = {
  getServiceReminders: async (filter: ServiceReminderFilter = {}) => {
    const queryParams = new URLSearchParams();
    if (filter.PageNumber)
      queryParams.append("PageNumber", filter.PageNumber.toString());
    if (filter.PageSize)
      queryParams.append("PageSize", filter.PageSize.toString());
    if (filter.Search) queryParams.append("Search", filter.Search);
    if (filter.SortBy) queryParams.append("SortBy", filter.SortBy);
    if (filter.SortDescending !== undefined)
      queryParams.append("SortDescending", filter.SortDescending.toString());
    const queryString = queryParams.toString();
    const { data } = await agent.get<{
      items: ServiceReminder[];
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    }>(`/api/ServiceReminders${queryString ? `?${queryString}` : ""}`);
    return data;
  },

  addWorkOrderToServiceReminder: async (
    serviceReminderId: number,
    workOrderId: number,
  ) => {
    const { data } = await agent.patch(
      `/api/ServiceReminders/${serviceReminderId}`,
      { serviceReminderID: serviceReminderId, workOrderID: workOrderId },
    );
    return data;
  },
};

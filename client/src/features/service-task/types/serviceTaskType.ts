import { ServiceTaskCategoryEnum } from "./serviceTaskEnum";

export interface ServiceTask {
  id: number;
  name: string;
  description?: string | null;
  estimatedLabourHours: number;
  estimatedCost: number;
  category: ServiceTaskCategoryEnum;
  isActive: boolean;
}

export interface ServiceTaskWithLabels extends Omit<ServiceTask, "category"> {
  category: number;
  categoryLabel: string;
  categoryEnum: ServiceTaskCategoryEnum;
}

export interface CreateServiceTaskCommand {
  name: string;
  description?: string | null;
  estimatedLabourHours: number;
  estimatedCost: number;
  category: ServiceTaskCategoryEnum;
  isActive: boolean;
}

export interface UpdateServiceTaskCommand {
  ServiceTaskID: number;
  name: string;
  description?: string | null;
  estimatedLabourHours: number;
  estimatedCost: number;
  category: ServiceTaskCategoryEnum;
  isActive: boolean;
}

export interface ServiceTaskFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}

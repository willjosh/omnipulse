import { ServiceTaskCategoryEnum } from "./serviceTaskEnum";

export interface ServiceTask {
  id: number;
  Name: string;
  Description?: string | null;
  EstimatedLabourHours: number;
  EstimatedCost: number;
  Category: ServiceTaskCategoryEnum;
  IsActive: boolean;
}

export interface ServiceTaskWithLabels extends Omit<ServiceTask, "Category"> {
  Category: number;
  CategoryLabel: string;
  CategoryEnum: ServiceTaskCategoryEnum;
}

export interface CreateServiceTaskCommand {
  Name: string;
  Description?: string | null;
  EstimatedLabourHours: number;
  EstimatedCost: number;
  Category: ServiceTaskCategoryEnum;
  IsActive: boolean;
}

export interface UpdateServiceTaskCommand {
  id: number;
  Name: string;
  Description?: string | null;
  EstimatedLabourHours: number;
  EstimatedCost: number;
  Category: ServiceTaskCategoryEnum;
  IsActive: boolean;
}

export interface ServiceTaskFilter {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  search?: string;
}

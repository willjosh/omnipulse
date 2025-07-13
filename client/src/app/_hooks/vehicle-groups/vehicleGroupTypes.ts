export interface VehicleGroup {
  id: number;
  Name: string;
  Description: string;
  IsActive: boolean;
}

export interface CreateVehicleGroupCommand {
  Name: string;
  Description: string;
  IsActive: boolean;
}

export interface UpdateVehicleGroupCommand {
  id: number;
  name: string;
  description: string;
  isActive: boolean;
}

export interface VehicleGroupFilter {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  search?: string;
  isActive?: boolean;
}

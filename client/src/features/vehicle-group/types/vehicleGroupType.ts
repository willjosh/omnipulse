export interface VehicleGroup {
  id: number;
  name: string;
  description: string;
  isActive: boolean;
}

export interface CreateVehicleGroupCommand {
  name: string;
  description?: string | null;
  isActive: boolean;
}

export interface UpdateVehicleGroupCommand {
  vehicleGroupId: number;
  name: string;
  description?: string | null;
  isActive: boolean;
}

export interface VehicleGroupFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}

export interface ServiceProgramVehicle {
  serviceProgramID: number;
  vehicleID: number;
  vehicleName: string;
  addedAt: string;
}

export interface AddVehicleToServiceProgramCommand {
  serviceProgramID: number;
  vehicleID: number;
}

export interface RemoveVehicleFromServiceProgramCommand {
  serviceProgramID: number;
  vehicleID: number;
}

export interface RemoveVehicleFromServiceProgramResponse {}

export interface ServiceProgramVehicleFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}

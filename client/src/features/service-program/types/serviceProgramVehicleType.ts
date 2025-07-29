// ServiceProgramVehicle type for get/query response
export interface ServiceProgramVehicle {
  serviceProgramID: number;
  vehicleID: number;
  vehicleName: string;
  addedAt: string;
}

// Command for adding a vehicle to a service program
export interface AddVehicleToServiceProgramCommand {
  serviceProgramID: number;
  vehicleID: number;
}

// Command for removing a vehicle from a service program
export interface RemoveVehicleFromServiceProgramCommand {
  serviceProgramID: number;
  vehicleID: number;
}

// Response type for removing a vehicle (empty object as per API spec)
export interface RemoveVehicleFromServiceProgramResponse {
  // Empty response as per API specification
}

// Filter for service program vehicles (PascalCase keys)
export interface ServiceProgramVehicleFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}

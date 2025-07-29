export interface VehicleStatus {
  id: number;
  name: string;
  color: string;
  isActive: boolean;
  vehicleCount: number;
}

export interface CreateVehicleStatusCommand {
  name: string;
  color: string;
}

export interface UpdateVehicleStatusCommand {
  id: number;
  name: string;
  color: string;
}

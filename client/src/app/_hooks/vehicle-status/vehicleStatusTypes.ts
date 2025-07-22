export interface VehicleStatus {
  id: number;
  name: string;
  label: string;
  color: string;
  isDefault: boolean;
  isActive: boolean;
}

export interface CreateVehicleStatusCommand {
  name: string;
  label: string;
  color: string;
  isDefault: boolean;
}

export interface UpdateVehicleStatusCommand {
  id: number;
  name: string;
  label: string;
  color: string;
  isDefault: boolean;
}

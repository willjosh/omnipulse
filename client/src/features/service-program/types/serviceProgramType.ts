import {
  ServiceSchedule,
  ServiceScheduleWithLabels,
} from "../../service-schedule/types/serviceScheduleType";

// ServiceProgram type for get/query
export interface ServiceProgram {
  id: number;
  name: string;
  description?: string | null;
  isActive: boolean;
  serviceScheduleCount: number;
  assignedVehicleCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface ServiceProgramDetails {
  id: number;
  name: string;
  description?: string | null;
  isActive: boolean;
  serviceSchedules: ServiceSchedule[];
  assignedVehiclesIDs: number[];
}

export interface ServiceProgramDetailsWithLabels
  extends Omit<ServiceProgramDetails, "serviceSchedules"> {
  serviceSchedules: ServiceScheduleWithLabels[];
}

// Command for creating a service program
export interface CreateServiceProgramCommand {
  name: string;
  description?: string | null;
  isActive: boolean;
}

// Command for updating a service program
export interface UpdateServiceProgramCommand {
  serviceProgramID: number;
  name: string;
  description?: string | null;
  isActive: boolean;
}

// Filter for service programs (PascalCase keys)
export interface ServiceProgramFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}

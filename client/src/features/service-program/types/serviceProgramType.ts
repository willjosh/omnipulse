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

export interface CreateServiceProgramCommand {
  name: string;
  description?: string | null;
  isActive: boolean;
}

export interface UpdateServiceProgramCommand {
  serviceProgramID: number;
  name: string;
  description?: string | null;
  isActive: boolean;
}

export interface ServiceProgramFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}

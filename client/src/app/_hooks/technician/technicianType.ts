export interface Technician {
  id: string;
  firstName: string;
  lastName: string;
  hireDate: string;
  isActive: boolean;
  email: string;
}

export interface CreateTechnicianCommand {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  hireDate: string;
  isActive: boolean;
}

export interface UpdateTechnicianCommand {
  id: string;
  firstName?: string | null;
  lastName?: string | null;
  hireDate?: string | null;
  isActive?: boolean | null;
}

export interface TechnicianFilter {
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
  Search?: string;
}

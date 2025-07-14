export interface Technician {
  id: string;
  FirstName: string;
  LastName: string;
  HireDate: string;
  IsActive: boolean;
  Email: string;
}

export interface CreateTechnicianCommand {
  Email: string;
  Password: string;
  FirstName: string;
  LastName: string;
  HireDate: string;
  IsActive: boolean;
}

export interface UpdateTechnicianCommand {
  id: string;
  FirstName?: string | null;
  LastName?: string | null;
  HireDate?: string | null;
  IsActive?: boolean | null;
}

export interface TechnicianFilter {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  search?: string;
}

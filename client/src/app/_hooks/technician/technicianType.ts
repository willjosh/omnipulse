export interface Technician {
  id: string;
  FirstName: string;
  LastName: string;
  HireDate: string; // ISO string
  IsActive: boolean;
  Email: string;
}

export interface CreateTechnicianCommand {
  Email: string;
  Password: string;
  FirstName: string;
  LastName: string;
  HireDate: string; // ISO string
  IsActive?: boolean; // optional, default true
}

export interface UpdateTechnicianCommand {
  Id: string;
  FirstName?: string;
  LastName?: string;
  HireDate?: string; // ISO string, optional
  IsActive?: boolean;
}

export interface TechnicianFilter {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  search?: string;
}

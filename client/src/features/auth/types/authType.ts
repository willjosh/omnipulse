export interface LoginCommand {
  email: string;
  password: string;
}

export interface RegisterCommand {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  hireDate: string;
  isActive: boolean;
}

export interface LoginResponse {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
  token: string;
  refreshToken: string;
  expires: string;
}

export type RegisterResponse = string;

export interface AuthUser {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: "FleetManager" | "Technician";
}

export interface AuthState {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}

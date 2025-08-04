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

// Login response - returns full user object with token
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

// Register response - returns only the user ID string
export type RegisterResponse = string;

// User object for internal use
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

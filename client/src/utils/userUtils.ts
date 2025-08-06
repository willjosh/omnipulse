import { AuthUser } from "@/features/auth/types/authType";

export const getUserInitials = (firstName?: string, lastName?: string) => {
  if (!firstName || !lastName) return "NN";
  return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
};

export const getUserDisplayName = (user?: AuthUser | null) => {
  if (!user?.firstName || !user?.lastName) return "No Name";
  return `${user.firstName} ${user.lastName}`;
};

export const getFormattedRole = (user?: AuthUser | null) => {
  if (!user?.role) return "No Role";
  return user.role === "FleetManager" ? "Fleet Manager" : user.role;
};

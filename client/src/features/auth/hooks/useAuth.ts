import { useMutation, useQueryClient } from "@tanstack/react-query";
import { authApi } from "../api/authApi";
import { AuthUser } from "../types/authType";
import { useAuthContext } from "../context/AuthContext";

export function useLogin() {
  const { login } = useAuthContext();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: authApi.login,
    onSuccess: data => {
      const user: AuthUser = {
        id: data.id,
        firstName: data.firstName,
        lastName: data.lastName,
        email: data.email,
        role: data.roles.includes("FleetManager")
          ? "FleetManager"
          : "Technician",
      };

      login(data.token, data.refreshToken, user);
      queryClient.invalidateQueries({ queryKey: ["user"] });
    },
  });
}

export function useRegister() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: authApi.register,
    onSuccess: (userId: string) => {
      queryClient.invalidateQueries({ queryKey: ["user"] });
    },
  });
}

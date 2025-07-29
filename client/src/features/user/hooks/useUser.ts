import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { userApi, transformUserData } from "../api/userApi";
import { User, UpdateUserProfileCommand, UserProfile } from "../types/userType";

export function useCurrentUser() {
  const { data, isPending, isError, isSuccess, error } = useQuery<UserProfile>({
    queryKey: ["user"],
    queryFn: async () => {
      try {
        const userData = await userApi.getCurrentUser();
        return transformUserData(userData);
      } catch (error) {
        console.error("Failed to fetch user profile:", error);
        throw error;
      }
    },
    staleTime: 2 * 60 * 1000,
    retry: 3,
  });

  return { user: data, isPending, isError, isSuccess, error };
}

export function useUpdateUserProfile() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (updateCommand: UpdateUserProfileCommand) => {
      try {
        const userData = await userApi.updateUserProfile(updateCommand);
        return transformUserData(userData);
      } catch (error) {
        console.error("Failed to update user profile:", error);
        throw error;
      }
    },
    onSuccess: async updatedUser => {
      queryClient.setQueryData(["user"], updatedUser);
      await queryClient.invalidateQueries({ queryKey: ["user"] });
    },
    onError: error => {
      console.error("Update user profile mutation failed:", error);
    },
  });
}

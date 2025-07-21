import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import { User, UpdateUserProfileCommand, UserProfile } from "./userTypes";

const transformUserData = (user: User): UserProfile => ({
  ...user,
  fullName: `${user.firstName} ${user.lastName}`,
  initials:
    `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase(),
});

export const useUser = () => {
  const queryClient = useQueryClient();

  // Fetch current user profile
  const {
    data: user,
    isLoading,
    error,
  } = useQuery<UserProfile>({
    queryKey: ["user"],
    queryFn: async () => {
      try {
        const response = await agent.get<User>("/user");
        return transformUserData(response.data);
      } catch (error) {
        console.error("Failed to fetch user profile:", error);
        throw error;
      }
    },
    staleTime: 2 * 60 * 1000,
    retry: 3,
  });

  // Update user profile mutation
  const updateUserProfileMutation = useMutation({
    mutationFn: async (updateCommand: UpdateUserProfileCommand) => {
      try {
        const response = await agent.put<User>("/user", updateCommand);
        return transformUserData(response.data);
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

  return { user, isLoading, error, updateUserProfileMutation };
};

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
  const { data: user, isLoading } = useQuery<UserProfile>({
    queryKey: ["user"],
    queryFn: async () => {
      const response = await agent.get<User>("user");
      return transformUserData(response.data);
    },
  });

  // Update user profile mutation
  const updateUserProfileMutation = useMutation({
    mutationFn: async (updateCommand: UpdateUserProfileCommand) => {
      const response = await agent.put("user", updateCommand);
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["user"] });
    },
  });

  return { user, isLoading, updateUserProfileMutation };
};

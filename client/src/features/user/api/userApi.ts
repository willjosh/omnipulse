import { agent } from "@/lib/axios/agent";
import { User, UpdateUserProfileCommand, UserProfile } from "../types/userType";

export const transformUserData = (user: User): UserProfile => ({
  ...user,
  fullName: `${user.firstName} ${user.lastName}`,
  initials:
    `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase(),
});

export const userApi = {
  getCurrentUser: async () => {
    const response = await agent.get<User>("/user");
    return response.data;
  },

  updateUserProfile: async (command: UpdateUserProfileCommand) => {
    const response = await agent.put<User>("/user", command);
    return response.data;
  },
};

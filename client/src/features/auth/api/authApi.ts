import { agent } from "@/lib/axios/agent";
import {
  LoginCommand,
  RegisterCommand,
  LoginResponse,
  RegisterResponse,
} from "../types/authType";

export const authApi = {
  login: async (command: LoginCommand): Promise<LoginResponse> => {
    const { data } = await agent.post<LoginResponse>(
      "/api/Auth/login",
      command,
    );
    return data;
  },

  register: async (command: RegisterCommand): Promise<RegisterResponse> => {
    const { data } = await agent.post<RegisterResponse>(
      "/api/Auth/register",
      command,
    );
    return data;
  },
};

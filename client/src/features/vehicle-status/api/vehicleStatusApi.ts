import { agent } from "@/lib/axios/agent";
import {
  VehicleStatus,
  CreateVehicleStatusCommand,
  UpdateVehicleStatusCommand,
} from "../types/vehicleStatusType";

export const vehicleStatusApi = {
  getVehicleStatuses: async () => {
    const response = await agent.get<any>("vehicleStatuses");
    return response.data.Items || response.data || [];
  },

  getVehicleStatus: async (id: number) => {
    const response = await agent.get<VehicleStatus>(`vehicleStatuses/${id}`);
    return response.data;
  },

  createVehicleStatus: async (command: CreateVehicleStatusCommand) => {
    const response = await agent.post("vehicleStatuses", command);
    return response.data;
  },

  updateVehicleStatus: async (command: UpdateVehicleStatusCommand) => {
    const response = await agent.put(`vehicleStatuses/${command.id}`, command);
    return response.data;
  },

  deleteVehicleStatus: async (id: number) => {
    const response = await agent.delete(`vehicleStatuses/${id}`);
    return response.data;
  },
};

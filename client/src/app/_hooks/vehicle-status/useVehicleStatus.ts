import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import {
  VehicleStatus,
  CreateVehicleStatusCommand,
  UpdateVehicleStatusCommand,
} from "./vehicleStatusTypes";

export const useVehicleStatuses = () => {
  const queryClient = useQueryClient();

  // Fetch vehicle statuses query
  const { data: vehicleStatuses = [], isLoading } = useQuery<VehicleStatus[]>({
    queryKey: ["vehicleStatuses"],
    queryFn: async () => {
      const response = await agent.get<any>("vehicleStatuses");
      return response.data.Items || response.data || [];
    },
  });

  // Create vehicle status mutation
  const createVehicleStatusMutation = useMutation({
    mutationFn: async (createCommand: CreateVehicleStatusCommand) => {
      const response = await agent.post("vehicleStatuses", createCommand);
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleStatuses"] });
    },
  });

  // Delete vehicle status mutation
  const deleteVehicleStatusMutation = useMutation({
    mutationFn: async (id: number) => {
      const response = await agent.delete(`vehicleStatuses/${id}`);
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleStatuses"] });
    },
  });

  // Update vehicle status mutation
  const updateVehicleStatusMutation = useMutation({
    mutationFn: async (updateVehicleStatus: UpdateVehicleStatusCommand) => {
      const response = await agent.put(
        `vehicleStatuses/${updateVehicleStatus.id}`,
        updateVehicleStatus,
      );
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleStatuses"] });
    },
  });

  return {
    vehicleStatuses,
    isLoading,
    createVehicleStatusMutation,
    deleteVehicleStatusMutation,
    updateVehicleStatusMutation,
  };
};

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import {
  VehicleGroup,
  CreateVehicleGroupCommand,
  UpdateVehicleGroupCommand,
} from "./vehicleGroupTypes";

export const useVehicleGroups = () => {
  const queryClient = useQueryClient();

  // Fetch vehicle groups query
  const { data: vehicleGroups = [], isLoading } = useQuery<VehicleGroup[]>({
    queryKey: ["vehicleGroups"],
    queryFn: async () => {
      const response = await agent.get<any>("vehicleGroups");
      return response.data.Items || [];
    },
  });

  // Create vehicle group mutation
  const createVehicleGroupMutation = useMutation({
    mutationFn: async (createCommand: CreateVehicleGroupCommand) => {
      const response = await agent.post("vehicleGroups", createCommand);
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleGroups"] });
    },
  });

  // Delete vehicle group mutation
  const deleteVehicleGroupMutation = useMutation({
    mutationFn: async (id: number) => {
      const response = await agent.delete(`vehicleGroups/${id}`);
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleGroups"] });
    },
  });

  // Update vehicle group mutation
  const updateVehicleGroupMutation = useMutation({
    mutationFn: async (updateVehicle: UpdateVehicleGroupCommand) => {
      const response = await agent.put(
        `vehicleGroups/${updateVehicle.id}`,
        updateVehicle,
      );
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["vehicleGroups"] });
    },
  });

  return {
    vehicleGroups,
    isLoading,
    createVehicleGroupMutation,
    deleteVehicleGroupMutation,
    updateVehicleGroupMutation,
  };
};

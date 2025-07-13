import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { agent } from "@/app/_lib/axios/agent";
import { VehicleGroup, CreateVehicleGroupCommand } from "./vehicleGroupTypes";

export const useVehicleGroups = () => {
  const queryClient = useQueryClient();

  // Fetch vehicle groups query
  const { data: vehicleGroups = [], isLoading } = useQuery<VehicleGroup[]>({
    queryKey: ["vehicleGroups"],
    queryFn: async () => {
      const response = await agent.get<VehicleGroup[]>("vehicleGroups");
      console.log(response.data);
      return response.data;
    },
  });

  // Create vehicle group mutation
  const createVehicleGroupMutation = useMutation({
    mutationFn: async (createCommand: CreateVehicleGroupCommand) => {
      const response = await agent.post("vehicleGroups", createCommand);
      return response.data;
    },
    onSuccess: async () => {
      // Invalidate and refetch vehicle groups
      await queryClient.invalidateQueries({ queryKey: ["vehicleGroups"] });
    },
  });

  return { vehicleGroups, isLoading, createVehicleGroupMutation };
};

import { useQuery, useQueryClient } from "@tanstack/react-query";
import { Vehicle } from "./vehicleType";
import { agent } from "@/app/_lib/axios/agent";

export const useVehicles = (id?: string) => {
  const queryClient = useQueryClient();

  // This hook fetches vehicles data from the API and returns it along with a loading state.
  const { data: vehicles, isPending } = useQuery<Vehicle[]>({
    queryKey: ["vehicles"],
    queryFn: async () => {
      const response = await agent.get("vehicles");
      return response.data;
    },
  });

  const { data: vehicle, isLoading: isLoadingVehicle } = useQuery({
    queryKey: ["vehicle", id],
    queryFn: async () => {
      const response = await agent.get(`vehicles/${id}`);
      return response.data;
    },
  });
};

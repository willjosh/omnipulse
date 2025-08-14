import { useQuery } from "@tanstack/react-query";
import { VehicleStatus } from "../types/vehicleStatusType";
import { getAllVehicleStatusOptions } from "../types/vehicleStatusEnum";
import { useVehicleStatusData } from "@/features/vehicle/hooks/useVehicles";

export function useVehicleStatuses() {
  const {
    activeVehicleCount,
    inactiveVehicleCount,
    maintenanceVehicleCount,
    outOfServiceVehicleCount,
    isVehicleStatusDataLoading,
    isError: isVehicleStatusDataError,
  } = useVehicleStatusData();

  const { data, isPending, isError, isSuccess, error } = useQuery<
    VehicleStatus[]
  >({
    queryKey: [
      "vehicleStatuses",
      activeVehicleCount,
      inactiveVehicleCount,
      maintenanceVehicleCount,
      outOfServiceVehicleCount,
    ],
    queryFn: async () => {
      const enumOptions = getAllVehicleStatusOptions();
      return enumOptions.map(option => {
        let vehicleCount = 0;

        switch (option.value) {
          case 1: // ACTIVE
            vehicleCount = activeVehicleCount;
            break;
          case 2: // MAINTENANCE
            vehicleCount = maintenanceVehicleCount;
            break;
          case 3: // OUT_OF_SERVICE
            vehicleCount = outOfServiceVehicleCount;
            break;
          case 4: // INACTIVE
            vehicleCount = inactiveVehicleCount;
            break;
        }

        return {
          id: option.value,
          name: option.label,
          color: option.color,
          isActive: true,
          vehicleCount,
        };
      });
    },
  });

  return {
    vehicleStatuses: data ?? [],
    isPending: isPending || isVehicleStatusDataLoading,
    isError: isError || isVehicleStatusDataError,
    isSuccess,
    error,
  };
}

export function useVehicleStatus(id: number) {
  const {
    activeVehicleCount,
    inactiveVehicleCount,
    maintenanceVehicleCount,
    outOfServiceVehicleCount,
    isVehicleStatusDataLoading,
    isError: isVehicleStatusDataError,
  } = useVehicleStatusData();

  const { data, isPending, isError, isSuccess, error } =
    useQuery<VehicleStatus>({
      queryKey: [
        "vehicleStatus",
        id,
        activeVehicleCount,
        inactiveVehicleCount,
        maintenanceVehicleCount,
        outOfServiceVehicleCount,
      ],
      queryFn: async () => {
        const enumOptions = getAllVehicleStatusOptions();
        const option = enumOptions.find(opt => opt.value === id);
        if (!option) {
          throw new Error(`Vehicle status with id ${id} not found`);
        }

        let vehicleCount = 0;
        switch (id) {
          case 1: // ACTIVE
            vehicleCount = activeVehicleCount;
            break;
          case 2: // MAINTENANCE
            vehicleCount = maintenanceVehicleCount;
            break;
          case 3: // OUT_OF_SERVICE
            vehicleCount = outOfServiceVehicleCount;
            break;
          case 4: // INACTIVE
            vehicleCount = inactiveVehicleCount;
            break;
        }

        return {
          id: option.value,
          name: option.label,
          color: option.color,
          isActive: true,
          vehicleCount,
        };
      },
      enabled: !!id,
    });

  return {
    vehicleStatus: data,
    isPending: isPending || isVehicleStatusDataLoading,
    isError: isError || isVehicleStatusDataError,
    isSuccess,
    error,
  };
}

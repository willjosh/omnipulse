import { VehicleStatusEnum } from "../vehicle/vehicleEnum";
export { VehicleStatusEnum } from "../vehicle/vehicleEnum";

export const VehicleStatusDisplayMap: Record<
  number,
  { name: string; label: string; color: string; isDefault: boolean }
> = {
  [1]: { name: "Active", label: "Active", color: "green", isDefault: true },
  [2]: { name: "In Shop", label: "In Shop", color: "orange", isDefault: false },
  [3]: {
    name: "Out of Service",
    label: "Out of Service",
    color: "red",
    isDefault: false,
  },
  [4]: { name: "Inactive", label: "Inactive", color: "blue", isDefault: false },
};

export const getStatusDisplayInfo = (statusId: number) => {
  return (
    VehicleStatusDisplayMap[statusId] || {
      name: `Status ${statusId}`,
      label: `Status ${statusId}`,
      color: "gray",
      isDefault: false,
    }
  );
};

export const getAllPossibleStatuses = () => {
  return Object.values(VehicleStatusEnum)
    .filter(value => typeof value === "number")
    .map(statusId => {
      const statusNum = statusId as number;
      return { id: statusNum, ...getStatusDisplayInfo(statusNum) };
    });
};

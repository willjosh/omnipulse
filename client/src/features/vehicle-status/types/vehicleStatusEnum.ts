import { VehicleStatusEnum } from "../../vehicle/types/vehicleEnum";
export { VehicleStatusEnum } from "../../vehicle/types/vehicleEnum";

export const VehicleStatusDisplayMap: Record<
  number,
  { name: string; color: string }
> = {
  [1]: { name: "Active", color: "green" },
  [2]: { name: "In Shop", color: "orange" },
  [3]: { name: "Out of Service", color: "red" },
  [4]: { name: "Inactive", color: "blue" },
  [5]: { name: "Under Maintenance", color: "purple" },
};

export const getStatusDisplayInfo = (statusId: number) => {
  return (
    VehicleStatusDisplayMap[statusId] || {
      name: `Status ${statusId}`,
      color: "gray",
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

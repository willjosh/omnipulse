import { VehicleStatus } from "@/app/_hooks/vehicle-status/vehicleStatusTypes";

interface Column<T> {
  key: keyof T | string;
  header: string;
  render?: (item: T) => React.ReactNode;
  sortable?: boolean;
  width?: string;
}

export const vehicleStatusTableColumns: Column<VehicleStatus>[] = [
  {
    key: "name",
    header: "Name",
    sortable: true,
    render: (status: VehicleStatus) => (
      <span
        className={`inline-flex px-3 py-1 text-sm font-medium rounded-full ${
          status.color === "green"
            ? "bg-green-100 text-green-800"
            : status.color === "orange"
              ? "bg-orange-100 text-orange-800"
              : status.color === "red"
                ? "bg-red-100 text-red-800"
                : status.color === "blue"
                  ? "bg-blue-100 text-blue-800"
                  : status.color === "purple"
                    ? "bg-purple-100 text-purple-800"
                    : "bg-gray-100 text-gray-800"
        }`}
      >
        {status.name}
      </span>
    ),
  },
  {
    key: "vehicleCount",
    header: "Usage",
    sortable: true,
    render: (status: VehicleStatus) => (
      <div className="text-sm text-gray-900">
        {status.vehicleCount}{" "}
        {status.vehicleCount === 1 ? "vehicle" : "vehicles"}
      </div>
    ),
  },
];

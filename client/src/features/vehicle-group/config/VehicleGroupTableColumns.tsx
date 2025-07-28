import { VehicleGroup } from "@/features/vehicle-group/types/vehicleGroupType";

interface Column<T> {
  key: keyof T | string;
  header: string;
  render?: (item: T) => React.ReactNode;
  sortable?: boolean;
  width?: string;
}

export const vehicleGroupTableColumns: Column<VehicleGroup>[] = [
  {
    key: "Name",
    header: "Name",
    sortable: true,
    render: (group: VehicleGroup) => (
      <div className="text-sm font-medium text-gray-900">{group.name}</div>
    ),
  },
  {
    key: "Description",
    header: "Description",
    sortable: false,
    render: (group: VehicleGroup) => (
      <div className="text-sm text-gray-500 max-w-xs truncate">
        {group.description || "No description"}
      </div>
    ),
  },
  {
    key: "IsActive",
    header: "Status",
    sortable: true,
    render: (group: VehicleGroup) => (
      <span
        className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
          group.isActive
            ? "bg-green-100 text-green-800"
            : "bg-red-100 text-red-800"
        }`}
      >
        {group.isActive ? "Active" : "Inactive"}
      </span>
    ),
  },
];

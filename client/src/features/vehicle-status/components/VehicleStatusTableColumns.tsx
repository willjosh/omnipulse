import { VehicleStatus } from "../types/vehicleStatusType";

interface Column<T> {
  key: keyof T | string;
  header: string;
  render?: (item: T) => React.ReactNode;
  sortable?: boolean;
  width?: string;
}

const getColorClasses = (color: string): string => {
  const colorMap: Record<string, string> = {
    green: "bg-green-100 text-green-800",
    emerald: "bg-emerald-100 text-emerald-800",
    teal: "bg-teal-100 text-teal-800",
    cyan: "bg-cyan-100 text-cyan-800",
    sky: "bg-sky-100 text-sky-800",
    blue: "bg-blue-100 text-blue-800",
    indigo: "bg-indigo-100 text-indigo-800",
    violet: "bg-violet-100 text-violet-800",
    purple: "bg-purple-100 text-purple-800",
    fuchsia: "bg-fuchsia-100 text-fuchsia-800",
    pink: "bg-pink-100 text-pink-800",
    rose: "bg-rose-100 text-rose-800",
    red: "bg-red-100 text-red-800",
    orange: "bg-orange-100 text-orange-800",
    amber: "bg-amber-100 text-amber-800",
    yellow: "bg-yellow-100 text-yellow-800",
    lime: "bg-lime-100 text-lime-800",
    slate: "bg-slate-100 text-slate-800",
    gray: "bg-gray-100 text-gray-800",
    zinc: "bg-zinc-100 text-zinc-800",
    neutral: "bg-neutral-100 text-neutral-800",
    stone: "bg-stone-100 text-stone-800",
  };

  return colorMap[color] || "bg-gray-100 text-gray-800";
};

export const vehicleStatusTableColumns: Column<VehicleStatus>[] = [
  {
    key: "name",
    header: "Name",
    sortable: true,
    render: (status: VehicleStatus) => (
      <span
        className={`inline-flex px-3 py-1 text-sm font-medium rounded-full ${getColorClasses(status.color)}`}
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

import React from "react";

const DEFAULT_COLOR_MAP: Record<string, string> = {
  Open: "bg-green-100 text-green-800 border-green-200",
  Resolved: "bg-gray-100 text-gray-600 border-gray-200",
  Active: "bg-green-100 text-green-800 border-green-200",
  Inactive: "bg-red-100 text-red-800 border-red-200",
  Pending: "bg-yellow-100 text-yellow-800 border-yellow-200",
  Archived: "bg-gray-200 text-gray-500 border-gray-300",
  "In Shop": "bg-yellow-100 text-yellow-800 border-yellow-200",
  "Out of Service": "bg-red-100 text-red-800 border-red-200",
};

interface StatusBadgeProps {
  status: string;
  className?: string;
  colorMap?: Record<string, string>;
}

const StatusBadge: React.FC<StatusBadgeProps> = ({
  status,
  className = "",
  colorMap = DEFAULT_COLOR_MAP,
}) => {
  const colorClass =
    colorMap[status] || "bg-gray-100 text-gray-600 border-gray-200";
  return (
    <span
      className={`inline-block px-3 py-0.5 rounded-full border text-xs font-semibold ${colorClass} ${className}`}
      aria-label={`Status: ${status}`}
    >
      {status}
    </span>
  );
};

export default StatusBadge;

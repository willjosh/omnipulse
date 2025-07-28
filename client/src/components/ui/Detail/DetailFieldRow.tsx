import React from "react";

interface DetailFieldRowProps {
  label: string;
  value?: React.ReactNode;
  className?: string;
  children?: React.ReactNode; // For more complex value rendering
  noBorder?: boolean;
}

const DetailFieldRow: React.FC<DetailFieldRowProps> = ({
  label,
  value,
  className = "",
  children,
  noBorder = false,
}) => (
  <div
    className={`flex justify-between items-center py-3 ${
      noBorder ? "" : "border-b border-gray-100"
    } ${className}`}
  >
    <span className="text-sm font-medium text-gray-600">{label}</span>
    <span className="text-sm text-gray-900">{value ?? children}</span>
  </div>
);

export default DetailFieldRow;

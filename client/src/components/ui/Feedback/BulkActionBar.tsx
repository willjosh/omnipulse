import React from "react";

interface BulkAction {
  label: string;
  onClick: () => void;
  icon?: React.ReactNode;
  disabled?: boolean;
}

interface BulkActionBarProps {
  selectedCount: number;
  onClearSelection: () => void;
  actions: BulkAction[];
  className?: string;
}

const BulkActionBar: React.FC<BulkActionBarProps> = ({
  selectedCount,
  onClearSelection,
  actions,
  className = "",
}) => {
  if (selectedCount === 0) return null;
  return (
    <div
      className={`w-full flex items-center justify-between px-4 py-2 bg-white border-b border-gray-200 shadow-sm z-10 ${className}`}
      role="region"
      aria-label="Bulk actions bar"
    >
      <div className="flex items-center gap-2">
        <span className="text-sm font-medium">{selectedCount} selected</span>
        <button
          type="button"
          onClick={onClearSelection}
          className="ml-2 text-xs text-blue-600 hover:underline focus:outline-none"
        >
          Clear selection
        </button>
      </div>
      <div className="flex items-center gap-2">
        {actions.map(action => (
          <button
            key={action.label}
            type="button"
            onClick={action.onClick}
            disabled={action.disabled}
            className={
              "inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold border border-gray-300 bg-gray-50 hover:bg-blue-50 focus:outline-none transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            }
          >
            {action.icon && <span className="mr-1">{action.icon}</span>}
            {action.label}
          </button>
        ))}
      </div>
    </div>
  );
};

export default BulkActionBar;

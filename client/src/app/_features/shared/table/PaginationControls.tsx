// /shared/PaginationControls.tsx
import React from "react";

interface PaginationControlsProps {
  currentPage: number;
  totalPages: number;
  totalItems: number;
  itemsPerPage: number;
  onPreviousPage: () => void;
  onNextPage: () => void;
  className?: string;
  showPageNumbers?: boolean;
  showItemCount?: boolean;
  size?: "sm" | "md" | "lg";
  variant?: "default" | "compact" | "detailed";
}

const PaginationControls: React.FC<PaginationControlsProps> = ({
  currentPage,
  totalPages,
  totalItems,
  itemsPerPage,
  onPreviousPage,
  onNextPage,
  className = "",
  showItemCount = true,
}) => {
  const startItem = totalItems === 0 ? 0 : (currentPage - 1) * itemsPerPage + 1;
  const endItem = Math.min(currentPage * itemsPerPage, totalItems);

  const getButtonClasses = (disabled: boolean) => {
    const baseClasses = `p-1 ${disabled ? "text-gray-300 cursor-not-allowed" : "text-gray-400 hover:text-gray-600"}`;
    return baseClasses;
  };

  return (
    <div className={`flex items-center gap-2 ${className}`}>
      {showItemCount && (
        <span className="text-sm text-gray-500">
          {totalItems === 0
            ? "0 - 0 of 0"
            : `${startItem} - ${endItem} of ${totalItems}`}
        </span>
      )}

      <div className="flex items-center">
        {/* Previous Page Button */}
        <button
          onClick={onPreviousPage}
          disabled={currentPage <= 1}
          className={getButtonClasses(currentPage <= 1)}
          aria-label="Previous page"
        >
          <svg
            className="size-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M15 19l-7-7 7-7"
            />
          </svg>
        </button>

        {/* Next Page Button */}
        <button
          onClick={onNextPage}
          disabled={currentPage >= totalPages}
          className={getButtonClasses(currentPage >= totalPages)}
          aria-label="Next page"
        >
          <svg
            className="size-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M9 5l7 7-7 7"
            />
          </svg>
        </button>
      </div>
    </div>
  );
};

export default PaginationControls;

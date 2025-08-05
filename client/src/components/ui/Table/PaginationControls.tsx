// /shared/PaginationControls.tsx
import React from "react";

interface PaginationControlsProps {
  currentPage: number;
  totalPages: number;
  totalItems: number;
  itemsPerPage: number;
  onPreviousPage: () => void;
  onNextPage: () => void;
  onPageChange?: (page: number) => void;
  onPageSizeChange?: (size: number) => void;
  className?: string;
  showPageNumbers?: boolean;
  showItemCount?: boolean;
  size?: "sm" | "md" | "lg";
  variant?: "default" | "compact" | "detailed";
  pageSizeOptions?: number[];
}

const DEFAULT_PAGE_SIZE_OPTIONS = [10, 25, 50, 100];

const PaginationControls: React.FC<PaginationControlsProps> = ({
  currentPage,
  totalPages,
  totalItems,
  itemsPerPage,
  onPreviousPage,
  onNextPage,
  onPageChange,
  onPageSizeChange,
  className = "",
  showItemCount = true,
  showPageNumbers = true,
  pageSizeOptions = DEFAULT_PAGE_SIZE_OPTIONS,
}) => {
  const startItem = totalItems === 0 ? 0 : (currentPage - 1) * itemsPerPage + 1;
  const endItem = Math.min(currentPage * itemsPerPage, totalItems);

  const getButtonClasses = (disabled: boolean, active?: boolean) => {
    let base = `px-2 py-1 rounded text-sm mx-0.5 ${disabled ? "text-gray-300 cursor-not-allowed" : "text-gray-600 hover:bg-gray-100"}`;
    if (active) base += " bg-blue-100 text-blue-700 font-semibold";
    return base;
  };

  // Generate page numbers with ellipsis logic
  const getPageNumbers = () => {
    const pages: (number | string)[] = [];
    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else {
      if (currentPage <= 4) {
        pages.push(1, 2, 3, 4, 5, "...", totalPages);
      } else if (currentPage >= totalPages - 3) {
        pages.push(
          1,
          "...",
          totalPages - 4,
          totalPages - 3,
          totalPages - 2,
          totalPages - 1,
          totalPages,
        );
      } else {
        pages.push(
          1,
          "...",
          currentPage - 1,
          currentPage,
          currentPage + 1,
          "...",
          totalPages,
        );
      }
    }
    return pages;
  };

  return (
    <div className={`flex items-center gap-4 ${className}`}>
      {showItemCount && (
        <span className="text-sm text-gray-500 whitespace-nowrap">
          {totalItems === 0
            ? "0 - 0 of 0"
            : `${startItem} - ${endItem} of ${totalItems}`}
        </span>
      )}

      {/* Page size selector */}
      {onPageSizeChange && (
        <div className="flex items-center gap-2 whitespace-nowrap">
          <span className="text-sm text-gray-500">Show</span>
          <select
            className="border border-gray-300 rounded-md px-3 py-1.5 text-sm text-gray-700 bg-white focus:outline-none min-w-[70px]"
            value={itemsPerPage}
            onChange={e => onPageSizeChange(Number(e.target.value))}
          >
            {pageSizeOptions.map(size => (
              <option key={size} value={size}>
                {size}
              </option>
            ))}
          </select>
          <span className="text-sm text-gray-500">per page</span>
        </div>
      )}

      {/* Separator */}
      {onPageSizeChange && showItemCount && (
        <div className="h-4 w-px bg-gray-300"></div>
      )}

      {/* Navigation Controls */}
      <div className="flex items-center gap-1">
        {/* First page */}
        {onPageChange && (
          <button
            onClick={() => onPageChange(1)}
            disabled={currentPage === 1}
            className={getButtonClasses(currentPage === 1)}
            aria-label="First page"
          >
            «
          </button>
        )}

        {/* Previous */}
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

        {/* Page numbers */}
        {showPageNumbers && onPageChange && (
          <div className="flex items-center">
            {getPageNumbers().map((page, idx) =>
              typeof page === "number" ? (
                <button
                  key={page}
                  onClick={() => onPageChange(page)}
                  className={getButtonClasses(false, page === currentPage)}
                  aria-current={page === currentPage ? "page" : undefined}
                >
                  {page}
                </button>
              ) : (
                <span key={"ellipsis-" + idx} className="px-1 text-gray-400">
                  …
                </span>
              ),
            )}
          </div>
        )}

        {/* Next */}
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

        {/* Last page */}
        {onPageChange && (
          <button
            onClick={() => onPageChange(totalPages)}
            disabled={currentPage === totalPages}
            className={getButtonClasses(currentPage === totalPages)}
            aria-label="Last page"
          >
            »
          </button>
        )}
      </div>
    </div>
  );
};

export default PaginationControls;

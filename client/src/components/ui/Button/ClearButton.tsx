import React from "react";

interface ClearButtonProps {
  onClick: () => void;
  className?: string;
  ariaLabel?: string;
}

const ClearButton: React.FC<ClearButtonProps> = ({
  onClick,
  className = "absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600 focus:outline-none focus:text-gray-600",
  ariaLabel = "Clear",
}) => {
  return (
    <button
      type="button"
      aria-label={ariaLabel}
      onClick={onClick}
      className={className}
    >
      <svg
        className="h-4 w-4"
        fill="none"
        stroke="currentColor"
        viewBox="0 0 24 24"
      >
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth={2}
          d="M6 18L18 6M6 6l12 12"
        />
      </svg>
    </button>
  );
};

export default ClearButton;

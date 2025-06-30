import React from "react";

interface OptionsButtonProps {
  onClick?: () => void;
  className?: string;
}

const OptionButton: React.FC<OptionsButtonProps> = ({
  onClick,
  className = "",
}) => {
  return (
    <button
      onClick={onClick}
      aria-label="More options"
      className={`text-gray-400 hover:text-gray-600 ${className}`}
    >
      <svg
        className="w-5 h-5"
        fill="currentColor"
        viewBox="0 0 20 20"
        aria-hidden="true"
      >
        <path d="M10 6a2 2 0 110-4 2 2 0 010 4zM10 12a2 2 0 110-4 2 2 0 010 4zM10 18a2 2 0 110-4 2 2 0 010 4z" />
      </svg>
    </button>
  );
};

export default OptionButton;

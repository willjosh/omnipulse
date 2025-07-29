import React from "react";

interface SecondaryButtonProps {
  onClick?: () => void;
  className?: string;
  children: React.ReactNode;
  type?: "button" | "submit" | "reset";
  disabled?: boolean; // Add this prop
}

const SecondaryButton: React.FC<SecondaryButtonProps> = ({
  onClick,
  className = "",
  children,
  type = "button",
  disabled = false, // Add default value
}) => {
  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled} // Pass to button element
      className={`px-6 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 ${
        disabled ? "opacity-50 cursor-not-allowed hover:bg-white" : ""
      } ${className}`}
    >
      {children}
    </button>
  );
};

export default SecondaryButton;

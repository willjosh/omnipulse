import React from "react";

interface PrimaryButtonProps {
  children: React.ReactNode;
  onClick?: () => void | Promise<void>;
  disabled?: boolean; // Add this line
  className?: string;
  type?: "button" | "submit" | "reset";
}

const PrimaryButton: React.FC<PrimaryButtonProps> = ({
  children,
  onClick,
  disabled = false, // Add default value
  className,
  type = "button",
}) => {
  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled} // Pass to button element
      className={`flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 ${className} ${disabled ? "opacity-50 cursor-not-allowed" : ""}`}
    >
      {children}
    </button>
  );
};

export default PrimaryButton;

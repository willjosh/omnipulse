import React from "react";

interface PrimaryButtonProps {
  onClick?: () => void;
  children: React.ReactNode;
  className?: string;
  type?: "button" | "submit" | "reset";
}

const PrimaryButton: React.FC<PrimaryButtonProps> = ({
  onClick,
  className,
  children,
  type = "button",
}) => {
  return (
    <button
      type={type}
      onClick={onClick}
      className={`flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 ${className}`}
    >
      {children}
    </button>
  );
};

export default PrimaryButton;

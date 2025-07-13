import React from "react";

interface FormFieldProps {
  label: string;
  htmlFor?: string;
  children: React.ReactNode;
  error?: string;
  required?: boolean;
  className?: string;
}

const FormField: React.FC<FormFieldProps> = ({
  label,
  htmlFor,
  children,
  error,
  required = false,
  className = "",
}) => (
  <div className={`flex flex-col gap-1 ${className}`}>
    <label htmlFor={htmlFor} className="font-medium text-gray-700">
      {label}
      {required && <span className="text-red-500 ml-1">*</span>}
    </label>
    {children}
    {error && <span className="text-sm text-red-500">{error}</span>}
  </div>
);

export default FormField;

import React from "react";

interface FormContainerProps {
  children: React.ReactNode;
  title?: string;
  description?: string;
  className?: string;
}

const FormContainer: React.FC<FormContainerProps> = ({
  children,
  title,
  description,
  className = "",
}) => (
  <section className={`bg-white p-6 rounded-lg shadow-sm ${className}`}>
    {title && <h2 className="text-xl font-semibold mb-2">{title}</h2>}
    {description && <p className="text-gray-500 mb-4">{description}</p>}
    <form className="space-y-4">{children}</form>
  </section>
);

export default FormContainer;

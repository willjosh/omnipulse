import React from "react";

interface HeaderBarProps {
  title: string;
  actions?: React.ReactNode;
  children?: React.ReactNode;
  className?: string;
}

const HeaderBar: React.FC<HeaderBarProps> = ({
  title,
  actions,
  children,
  className = "",
}) => {
  return (
    <header className={`flex flex-col gap-2 mb-6 ${className}`}>
      <div className="flex items-center justify-between gap-4">
        <h1 className="text-2xl font-bold text-gray-800">{title}</h1>
        {actions && <div className="flex-shrink-0">{actions}</div>}
      </div>
      {children && <div>{children}</div>}
    </header>
  );
};

export default HeaderBar;

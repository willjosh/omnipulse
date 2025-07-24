import React from "react";
import Breadcrumbs, { BreadcrumbItem } from "../../shared/layout/Breadcrumbs";

interface IssueHeaderProps {
  title: string;
  breadcrumbs: BreadcrumbItem[];
  actions?: React.ReactNode;
  className?: string;
  children?: React.ReactNode;
}

const IssueHeader: React.FC<IssueHeaderProps> = ({
  title,
  breadcrumbs,
  actions,
  className = "",
  children,
}) => (
  <div
    className={`sticky top-16 z-30 bg-white border-b border-gray-50 shadow-sm w-full ${className}`}
  >
    <div className="px-6 pt-4 pb-3">
      <Breadcrumbs items={breadcrumbs} className="mb-2" />
      <div className="flex items-center justify-between min-h-[48px]">
        <h1 className="text-[1.5rem] font-semibold leading-tight text-gray-900">
          {title}
        </h1>
        {actions && <div className="flex gap-2">{actions}</div>}
      </div>
      {children}
    </div>
  </div>
);

export default IssueHeader;

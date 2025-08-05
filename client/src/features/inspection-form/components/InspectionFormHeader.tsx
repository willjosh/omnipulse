import React from "react";
import Breadcrumbs, {
  BreadcrumbItem,
} from "@/components/ui/Layout/Breadcrumbs";

interface InspectionFormHeaderProps {
  title: string;
  description?: string;
  showDescription?: boolean;
  breadcrumbs: BreadcrumbItem[];
  actions?: React.ReactNode;
  className?: string;
  children?: React.ReactNode;
}

const InspectionFormHeader: React.FC<InspectionFormHeaderProps> = ({
  title,
  description,
  showDescription = false,
  breadcrumbs,
  actions,
  className = "",
  children,
}) => (
  <div
    className={`sticky top-0 z-30 bg-white border-b border-gray-50 shadow-sm w-full ${className}`}
  >
    <div className="px-6 pt-4 pb-3">
      <Breadcrumbs items={breadcrumbs} className="mb-2" />
      <div className="flex items-start justify-between min-h-[48px]">
        <div className="flex-1">
          <h1 className="text-[1.5rem] font-semibold leading-tight text-gray-900">
            {title}
          </h1>
          {showDescription && (
            <div className="mt-1">
              <span className="text-sm font-medium text-gray-700">
                Description:
              </span>
              <p className="text-sm text-gray-600 mt-1">{description || "-"}</p>
            </div>
          )}
        </div>
        {actions && <div className="flex gap-2">{actions}</div>}
      </div>
      {children}
    </div>
  </div>
);

export default InspectionFormHeader;

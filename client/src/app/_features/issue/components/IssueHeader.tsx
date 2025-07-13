import React from "react";
import Breadcrumbs, { BreadcrumbItem } from "../../shared/layout/Breadcrumbs";
import PrimaryButton from "../../shared/button/PrimaryButton";
import SecondaryButton from "../../shared/button/SecondaryButton";

interface IssueHeaderProps {
  title: string;
  breadcrumbs: BreadcrumbItem[];
  onCancel?: () => void;
  onSave?: () => void;
  cancelText?: string;
  saveText?: string;
  isSaving?: boolean;
  className?: string;
}

const IssueHeader: React.FC<IssueHeaderProps> = ({
  title,
  breadcrumbs,
  onCancel,
  onSave,
  cancelText = "Cancel",
  saveText = "Save Issue",
  isSaving = false,
  className = "",
}) => {
  return (
    <div
      className={`sticky top-16 z-30 bg-white border-b shadow-sm ${className}`}
    >
      <div className="px-4 pt-3 pb-2">
        <Breadcrumbs items={breadcrumbs} className="mb-2" />
        <div className="flex items-center justify-between min-h-[48px]">
          <h1 className="text-[1.5rem] font-semibold leading-tight text-gray-900">
            {title}
          </h1>
          <div className="flex gap-2">
            <SecondaryButton onClick={onCancel}>{cancelText}</SecondaryButton>
            <PrimaryButton onClick={onSave} disabled={isSaving}>
              {isSaving ? "Saving..." : saveText}
            </PrimaryButton>
          </div>
        </div>
      </div>
    </div>
  );
};

export default IssueHeader;

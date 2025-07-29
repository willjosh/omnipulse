import React from "react";
import { FilterBar } from "@/components/ui";
import {
  IssueCategoryEnum,
  IssueStatusEnum,
  PriorityLevelEnum,
} from "../types/issueEnum";

interface IssueListFiltersProps {
  searchValue: string;
  onSearchChange: (value: string) => void;
}

const statusOptions = [
  { value: "", label: "All Statuses" },
  { value: IssueStatusEnum.OPEN.toString(), label: "Open" },
  { value: IssueStatusEnum.IN_PROGRESS.toString(), label: "In Progress" },
  { value: IssueStatusEnum.RESOLVED.toString(), label: "Resolved" },
  { value: IssueStatusEnum.CLOSED.toString(), label: "Closed" },
  { value: IssueStatusEnum.CANCELLED.toString(), label: "Cancelled" },
];

const priorityOptions = [
  { value: "", label: "All Priorities" },
  { value: PriorityLevelEnum.LOW.toString(), label: "Low" },
  { value: PriorityLevelEnum.MEDIUM.toString(), label: "Medium" },
  { value: PriorityLevelEnum.HIGH.toString(), label: "High" },
  { value: PriorityLevelEnum.CRITICAL.toString(), label: "Critical" },
];

const categoryOptions = [
  { value: "", label: "All Categories" },
  { value: IssueCategoryEnum.ENGINE.toString(), label: "Engine" },
  { value: IssueCategoryEnum.TRANSMISSION.toString(), label: "Transmission" },
  { value: IssueCategoryEnum.BRAKES.toString(), label: "Brakes" },
  { value: IssueCategoryEnum.ELECTRICAL.toString(), label: "Electrical" },
  { value: IssueCategoryEnum.BODY.toString(), label: "Body" },
  { value: IssueCategoryEnum.TIRES.toString(), label: "Tires" },
  { value: IssueCategoryEnum.HVAC.toString(), label: "HVAC" },
  { value: IssueCategoryEnum.OTHER.toString(), label: "Other" },
];

export const IssueListFilters: React.FC<IssueListFiltersProps> = ({
  searchValue,
  onSearchChange,
}) => {
  return (
    <div className="flex flex-wrap gap-3 items-center mb-4">
      <FilterBar
        searchValue={searchValue}
        onSearchChange={onSearchChange}
        searchPlaceholder="Search issues..."
        onFilterChange={() => {}}
      />
    </div>
  );
};

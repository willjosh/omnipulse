import React from "react";
import { FilterBar } from "@/components/ui";

interface IssueListFiltersProps {
  searchValue: string;
  onSearchChange: (value: string) => void;
}

export const IssueListFilters: React.FC<IssueListFiltersProps> = ({
  searchValue,
  onSearchChange,
}) => {
  return (
    <div className="flex flex-wrap gap-3 items-center mb-4">
      <FilterBar
        searchValue={searchValue}
        onSearchChange={onSearchChange}
        searchPlaceholder="Search"
        onFilterChange={() => {}}
      />
    </div>
  );
};

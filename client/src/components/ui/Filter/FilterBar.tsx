import React from "react";
import SearchInput from "./SearchInput";

interface FilterBarProps {
  searchValue: string;
  onSearchChange: (value: string) => void;
  searchPlaceholder?: string;
  onFilterChange: (key: string, value: string) => void;
  className?: string;
}

const FilterBar: React.FC<FilterBarProps> = ({
  searchValue,
  onSearchChange,
  searchPlaceholder = "Search...",
  className = "",
}) => {
  return (
    <div className={`flex items-center justify-center ${className}`}>
      <div className="flex items-center gap-x-3">
        <SearchInput
          value={searchValue}
          onChange={onSearchChange}
          placeholder={searchPlaceholder}
          inputClassName="bg-white"
        />
      </div>
    </div>
  );
};

export default FilterBar;

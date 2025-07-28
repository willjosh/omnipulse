import React from "react";
import SearchInput from "./SearchInput";
// import DropdownFilter from "./DropdownFilter";

interface FilterOption {
  value: string;
  label: string;
}

interface Filter {
  key: string;
  placeholder: string;
  options: FilterOption[];
  value: string;
}

interface FilterBarProps {
  searchValue: string;
  onSearchChange: (value: string) => void;
  searchPlaceholder?: string;
  // filters?: Filter[];
  onFilterChange: (key: string, value: string) => void;
  className?: string;
}

const FilterBar: React.FC<FilterBarProps> = ({
  searchValue,
  onSearchChange,
  searchPlaceholder = "Search...",
  // filters = [],
  // onFilterChange,
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
        {/* {filters.map(filter => (
          <DropdownFilter
            key={filter.key}
            value={filter.value}
            onChange={value => onFilterChange(filter.key, value)}
            placeholder={filter.placeholder}
            options={filter.options}
          />
        ))} */}
      </div>
    </div>
  );
};

export default FilterBar;

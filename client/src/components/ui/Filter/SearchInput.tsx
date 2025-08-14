import React, { useCallback, useEffect, useRef, useState } from "react";
import { SearchIcon } from "@/components/ui/Icons";
import { ClearButton } from "@/components/ui/Button";

interface SearchBarProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  containerClassName?: string;
  inputClassName?: string;
  debounceMs?: number;
  ariaLabel?: string;
  fullWidth?: boolean;
}

const SearchInput: React.FC<SearchBarProps> = ({
  value,
  onChange,
  placeholder = "Search...",
  containerClassName = "",
  inputClassName = "",
  debounceMs = 0,
  ariaLabel = "Search",
  fullWidth = false,
}) => {
  const [internalValue, setInternalValue] = useState(value);
  const debounceTimeout = useRef<number | null>(null);

  useEffect(() => {
    setInternalValue(value);
  }, [value]);

  useEffect(() => {
    if (debounceMs > 0) {
      if (debounceTimeout.current) clearTimeout(debounceTimeout.current);
      debounceTimeout.current = window.setTimeout(() => {
        if (internalValue !== value) onChange(internalValue);
      }, debounceMs);
      return () => {
        if (debounceTimeout.current) clearTimeout(debounceTimeout.current);
      };
    } else {
      onChange(internalValue);
    }
  }, [internalValue]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setInternalValue(e.target.value);
    if (debounceMs === 0) {
      onChange(e.target.value);
    }
  };

  const handleClear = useCallback(() => {
    setInternalValue("");
    onChange("");
  }, [onChange]);

  return (
    <div
      className={`relative ${fullWidth ? "w-full" : ""} ${containerClassName}`}
    >
      <input
        type="text"
        placeholder={placeholder}
        value={internalValue}
        onChange={handleInputChange}
        aria-label={ariaLabel}
        className={`px-4 py-2 pl-10 pr-12 border border-gray-300 rounded-3xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent truncate ${fullWidth ? "w-full" : "w-48"} ${inputClassName}`}
      />
      <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
        <SearchIcon />
      </div>
      {internalValue && (
        <ClearButton onClick={handleClear} ariaLabel="Clear search" />
      )}
    </div>
  );
};

export default SearchInput;

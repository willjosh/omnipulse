import React, { useCallback, useEffect, useRef, useState } from "react";

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

  // Sync internal value with external value
  useEffect(() => {
    setInternalValue(value);
  }, [value]);

  // Debounced onChange
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
        className={`px-4 py-2 pl-10 border border-gray-300 rounded-3xl text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent ${fullWidth ? "w-full" : "w-48"} ${inputClassName}`}
      />
      {/* Search Icon */}
      <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
        <svg
          className="h-4 w-4 text-gray-400"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={2}
            d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
          />
        </svg>
      </div>
      {/* Clear Button */}
      {internalValue && (
        <button
          type="button"
          aria-label="Clear search"
          onClick={handleClear}
          className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600 focus:outline-none"
        >
          <svg
            className="h-4 w-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </button>
      )}
    </div>
  );
};

export default SearchInput;

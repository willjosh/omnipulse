/**
 * Utility functions for handling empty values consistently across the application
 */

/**
 * Returns an em dash (—) if the value is null, undefined, or empty string
 * Otherwise returns the original value
 */
export const formatEmptyValue = (value: any): React.ReactNode => {
  if (value === null || value === undefined || value === "") {
    return <span className="text-gray-400">—</span>;
  }
  return value;
};

/**
 * Returns an em dash (—) if the value is null, undefined, empty string, or "Unknown"
 * Otherwise returns the original value
 */
export const formatEmptyValueWithUnknown = (value: any): React.ReactNode => {
  if (
    value === null ||
    value === undefined ||
    value === "" ||
    value === "Unknown" ||
    value === "Unassigned"
  ) {
    return <span className="text-gray-400">—</span>;
  }
  return value;
};

/**
 * Returns an em dash (—) if the value is null, undefined, empty string, or "Unknown"
 * Otherwise returns the original value as a string
 */
export const formatEmptyValueAsString = (value: any): string => {
  if (
    value === null ||
    value === undefined ||
    value === "" ||
    value === "Unknown" ||
    value === "Unassigned"
  ) {
    return "—";
  }
  return String(value);
};

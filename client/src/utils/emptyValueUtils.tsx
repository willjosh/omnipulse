export const formatEmptyValue = (value: any): React.ReactNode => {
  if (value === null || value === undefined || value === "") {
    return <span className="text-gray-400">—</span>;
  }
  return value;
};

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

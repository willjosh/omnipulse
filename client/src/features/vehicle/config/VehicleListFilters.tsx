export const vehicleFilterConfig = [
  {
    key: "vehicleType",
    placeholder: "Vehicle Type",
    options: [
      { value: "City Bus", label: "City Bus" },
      { value: "Tour Bus", label: "Tour Bus" },
      { value: "School Bus", label: "School Bus" },
      { value: "Minibus", label: "Minibus" },
    ],
  },
  {
    key: "vehicleStatus",
    placeholder: "Vehicle Status",
    options: [
      { value: "Active", label: "Active" },
      { value: "Inactive", label: "Inactive" },
      { value: "Maintenance", label: "Maintenance" },
      { value: "Out of Service", label: "Out of Service" },
    ],
  },
  {
    key: "location",
    placeholder: "Location",
    options: [
      { value: "Sydney", label: "Sydney" },
      { value: "Melbourne", label: "Melbourne" },
      { value: "Brisbane", label: "Brisbane" },
      { value: "Canberra", label: "Canberra" },
    ],
  },
];

export const vehicleTabConfig = [
  { key: "all", label: "All" },
  { key: "assigned", label: "Assigned" },
  { key: "unassigned", label: "Unassigned" },
  { key: "archived", label: "Archived" },
];

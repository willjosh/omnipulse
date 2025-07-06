export const getStatusColor = (status: string) => {
  switch (status) {
    case "Active":
      return "text-green-600 bg-green-50";
    case "In Shop":
      return "text-yellow-600 bg-yellow-50";
    case "Inactive":
      return "text-blue-600 bg-blue-50";
    case "Out of Service":
      return "text-red-600 bg-red-50";
    default:
      return "text-gray-600 bg-gray-50";
  }
};

export const getStatusDot = (status: string) => {
  switch (status) {
    case "Active":
      return "bg-green-500";
    case "In Shop":
      return "bg-yellow-500";
    case "Inactive":
      return "bg-blue-500";
    case "Out of Service":
      return "bg-red-500";
    default:
      return "bg-gray-500";
  }
};

export const getVehicleIcon = (type: string) => {
  switch (type.toLowerCase()) {
    case "city bus":
    case "tour bus":
    case "school bus":
      return "🚌";
    case "minibus":
      return "🚐";
    default:
      return "🚗";
  }
};

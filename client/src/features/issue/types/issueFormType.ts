export interface IssueDetailsFormProps {
  value: {
    vehicleID: string;
    priorityLevel: string;
    reportedDate: string;
    title: string;
    description: string;
    status: string;
    category: string;
    reportedByUserID?: string;
  };
  errors: { [key: string]: string };
  onChange: (field: string, value: string) => void;
  disabled?: boolean;
  showStatus?: boolean;
  statusEditable?: boolean;
}

export type VehicleOption = { value: string; label: string };

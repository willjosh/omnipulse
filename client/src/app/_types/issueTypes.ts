export interface IssueDetailsFormProps {
  value: {
    VehicleID: string;
    PriorityLevel: string;
    ReportedDate: string;
    Title: string;
    Description: string;
    Status: string;
    ReportedByUserID: string;
    Category: string;
  };
  errors: { [key: string]: string };
  onChange: (field: string, value: string) => void;
  disabled?: boolean;
  showStatus?: boolean;
  statusEditable?: boolean;
}

// Utilities for issue form state, validation, and mapping

export interface IssueFormState {
  // For future user stories:
  VehicleID: string; // Relating an issue to a vehicle
  PriorityLevel: string; // Setting a priority level
  ReportedDate: string; // System auto-generates timestamp
  Description: string; // Updating a detailed and formatted description
  Category: string; // Selecting an issue category
  Status: string; // Selecting a status category
  // Active fields:
  Title: string;
  ReportedByUserID: string;
}

export function validateIssueForm(form: IssueFormState) {
  const errors: { [key: string]: string } = {};
  // if (!form.VehicleID) errors.VehicleID = "Vehicle is required";
  // if (!form.PriorityLevel) errors.PriorityLevel = "Priority is required";
  // if (!form.ReportedDate) errors.ReportedDate = "Reported date is required";
  if (!form.Title) errors.Title = "Summary is required";
  // if (!form.Description) errors.Description = "Description is required";
  // if (!form.Category) errors.Category = "Category is required";
  // if (!form.Status) errors.Status = "Status is required";
  if (!form.ReportedByUserID)
    errors.ReportedByUserID = "Reported By is required";
  return errors;
}

export function mapFormToCreateIssueCommand(form: IssueFormState) {
  return {
    // VehicleID: Number(form.VehicleID),
    // PriorityLevel: Number(form.PriorityLevel),
    // ReportedDate: form.ReportedDate,
    Title: form.Title,
    // Description: form.Description,
    // Category: Number(form.Category),
    // Status: Number(form.Status),
    ReportedByUserID: form.ReportedByUserID,
  };
}

export const emptyIssueFormState: IssueFormState = {
  // For future user stories:
  VehicleID: "",
  PriorityLevel: "",
  ReportedDate: "",
  Description: "",
  Category: "",
  Status: "",
  // Active fields:
  Title: "",
  ReportedByUserID: "",
};

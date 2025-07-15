// Utilities for issue form state, validation, and mapping

export interface IssueFormState {
  VehicleID: string; // Relating an issue to a vehicle
  PriorityLevel: string; // Setting a priority level
  ReportedDate: string; // System auto-generates timestamp
  Description: string; // Updating a detailed and formatted description
  Category: string; // Selecting an issue category
  Status: string; // Selecting a status category
  Title: string;
  ReportedByUserID: string;
  // Add resolution fields
  ResolutionNotes?: string;
  ResolvedDate?: string;
  ResolvedByUserID?: string;
}

export function validateIssueForm(form: IssueFormState) {
  const errors: { [key: string]: string } = {};
  if (!form.VehicleID) errors.VehicleID = "Vehicle is required";
  if (!form.PriorityLevel) errors.PriorityLevel = "Priority is required";
  if (!form.Title) errors.Title = "Summary is required";
  if (!form.Category) errors.Category = "Category is required";
  if (!form.ReportedByUserID)
    errors.ReportedByUserID = "Reported By is required";
  return errors;
}

export function mapFormToCreateIssueCommand(form: IssueFormState) {
  return {
    VehicleID: Number(form.VehicleID),
    PriorityLevel: Number(form.PriorityLevel),
    ReportedDate: form.ReportedDate || null,
    Title: form.Title,
    Description: form.Description,
    Category: Number(form.Category),
    Status: Number(form.Status),
    ReportedByUserID: form.ReportedByUserID,
  };
}

export function mapFormToUpdateIssueCommand(form: IssueFormState, id: number) {
  function toISOorNull(dateStr: string | undefined): string | null {
    if (!dateStr) return null;
    const d = new Date(dateStr);
    return isNaN(d.getTime())
      ? null
      : d.toISOString().replace(/\.\d{3}Z$/, "Z");
  }
  return {
    id,
    VehicleID: Number(form.VehicleID),
    PriorityLevel: Number(form.PriorityLevel),
    ReportedDate: toISOorNull(form.ReportedDate),
    Title: form.Title,
    Description: form.Description,
    Category: Number(form.Category),
    Status: Number(form.Status),
    ReportedByUserID: form.ReportedByUserID,
    ResolutionNotes: form.ResolutionNotes || null,
    ResolvedDate: toISOorNull(form.ResolvedDate),
    ResolvedByUserID: form.ResolvedByUserID || null,
  };
}

export const emptyIssueFormState: IssueFormState = {
  VehicleID: "",
  PriorityLevel: "",
  ReportedDate: "",
  Description: "",
  Category: "",
  Status: "1",
  Title: "",
  ReportedByUserID: "",
  // Add resolution fields
  ResolutionNotes: "",
  ResolvedDate: "",
  ResolvedByUserID: "",
};

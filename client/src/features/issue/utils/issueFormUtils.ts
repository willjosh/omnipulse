export interface IssueFormState {
  vehicleID: string;
  priorityLevel: string;
  reportedDate: string;
  description: string;
  category: string;
  status: string;
  title: string;
  reportedByUserID?: string; // Optional for editing existing issues
  resolutionNotes?: string;
  resolvedDate?: string;
  resolvedByUserID?: string;
}

export function validateIssueForm(form: IssueFormState) {
  const errors: { [key: string]: string } = {};
  if (!form.vehicleID) errors.vehicleID = "Vehicle is required";
  if (!form.priorityLevel) errors.priorityLevel = "Priority is required";
  if (!form.title) errors.title = "Summary is required";
  if (!form.category) errors.category = "Category is required";

  // If status is RESOLVED, resolution notes, resolved date, and resolved by user are required
  if (
    form.status === "3" &&
    (!form.resolutionNotes?.trim() ||
      !form.resolvedDate ||
      !form.resolvedByUserID)
  ) {
    if (!form.resolutionNotes?.trim()) {
      errors.resolutionNotes =
        "Resolution notes are required when resolving an issue";
    }
    if (!form.resolvedDate) {
      errors.resolvedDate = "Resolved date is required when resolving an issue";
    }
    if (!form.resolvedByUserID) {
      errors.resolvedByUserID =
        "Resolved by user is required when resolving an issue";
    }
  }

  return errors;
}

export function mapFormToCreateIssueCommand(form: IssueFormState) {
  return {
    vehicleID: Number(form.vehicleID),
    priorityLevel: Number(form.priorityLevel),
    reportedDate: form.reportedDate || null,
    title: form.title,
    description: form.description,
    category: Number(form.category),
    status: Number(form.status),
    reportedByUserID: "",
  };
}

export function mapFormToUpdateIssueCommand(form: IssueFormState, id: number) {
  const command = {
    issueID: id,
    vehicleID: Number(form.vehicleID),
    priorityLevel: Number(form.priorityLevel),
    reportedDate: form.reportedDate || null,
    title: form.title,
    description: form.description,
    category: Number(form.category),
    status: Number(form.status),
    reportedByUserID: form.reportedByUserID || "",
    resolutionNotes: form.resolutionNotes || null,
    resolvedDate: form.resolvedDate || null,
    resolvedByUserID: form.resolvedByUserID || null,
  };

  return command;
}

export const emptyIssueFormState: IssueFormState = {
  vehicleID: "",
  priorityLevel: "",
  reportedDate: "",
  description: "",
  category: "",
  status: "1",
  title: "",
  reportedByUserID: "",
  resolutionNotes: "",
  resolvedDate: "",
  resolvedByUserID: "",
};

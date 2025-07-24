// Utilities for issue form state, validation, and mapping
import { toISOorNull } from "@/app/_utils/dateTimeUtils";

export interface IssueFormState {
  vehicleID: string;
  priorityLevel: string;
  reportedDate: string;
  description: string;
  category: string;
  status: string;
  title: string;
  reportedByUserID: string;
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
  if (!form.reportedByUserID)
    errors.reportedByUserID = "Reported By is required";
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
    reportedByUserID: form.reportedByUserID,
  };
}

export function mapFormToUpdateIssueCommand(form: IssueFormState, id: number) {
  return {
    issueID: id,
    vehicleID: Number(form.vehicleID),
    priorityLevel: Number(form.priorityLevel),
    reportedDate: toISOorNull(form.reportedDate),
    title: form.title,
    description: form.description,
    category: Number(form.category),
    status: Number(form.status),
    reportedByUserID: form.reportedByUserID,
    resolutionNotes: form.resolutionNotes || null,
    resolvedDate: toISOorNull(form.resolvedDate),
    resolvedByUserID: form.resolvedByUserID || null,
  };
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

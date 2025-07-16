// Utilities for issue form state, validation, and mapping
import { toISOorNull } from "@/app/_utils/dateTimeUtils";

export interface IssueFormState {
  VehicleID: string;
  PriorityLevel: string;
  ReportedDate: string;
  Description: string;
  Category: string;
  Status: string;
  Title: string;
  ReportedByUserID: string;
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
  ResolutionNotes: "",
  ResolvedDate: "",
  ResolvedByUserID: "",
};

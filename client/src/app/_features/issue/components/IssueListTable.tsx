import React from "react";
import { DataTable } from "../../shared/table/";

// Define the type for a single issue row (should match the data from useIssues)
export interface IssueRow {
  id: string;
  issueNumber: string;
  title: string;
  vehicle: string;
  category: string;
  priority: string;
  status: string;
  reportedBy: string;
  reportedDate: string;
  assignedTo: string;
}

interface IssueListTableProps {
  issues: IssueRow[];
  isLoading?: boolean;
  emptyState?: React.ReactNode;
  onRowClick?: (row: IssueRow) => void;
}

const columns = [
  { key: "issueNumber", header: "Issue #" },
  { key: "title", header: "Title", width: "280px" },
  { key: "vehicle", header: "Vehicle", width: "180px" },
  { key: "category", header: "Category" },
  { key: "priority", header: "Priority" },
  { key: "status", header: "Status" },
  { key: "reportedBy", header: "Reported By", width: "150px" },
  { key: "reportedDate", header: "Reported Date", width: "190px" },
  { key: "assignedTo", header: "Assigned To", width: "150px" },
];

export const IssueListTable: React.FC<IssueListTableProps> = ({
  issues,
  isLoading,
  emptyState,
  onRowClick,
}) => {
  // For now, selection is not implemented, so pass empty arrays and no-ops
  return (
    <DataTable
      columns={columns}
      data={issues}
      selectedItems={[]}
      onSelectItem={() => {}}
      onSelectAll={() => {}}
      getItemId={item => item.id}
      loading={isLoading}
      showActions={false}
      emptyState={emptyState}
      onRowClick={onRowClick}
    />
  );
};

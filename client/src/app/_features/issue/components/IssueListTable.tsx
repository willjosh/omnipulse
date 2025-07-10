import React from "react";
import { DataTable } from "../../shared/table/";

// Define the type for a single issue row (should match the data from useIssues)
export interface IssueRow {
  id: string;
  priority: string;
  assetName: string;
  assetRecordType: string;
  number: string;
  summary: string;
  status: string;
  source: string;
  reportedDate: string;
  assigned: string;
  labels: string;
  watchers: string;
  comments: string;
}

interface IssueListTableProps {
  issues: IssueRow[];
  isLoading?: boolean;
}

const columns = [
  { key: "priority", header: "Priority" },
  { key: "assetName", header: "Asset Name" },
  { key: "assetRecordType", header: "Asset Record Type" },
  { key: "number", header: "Issue #" },
  { key: "summary", header: "Summary" },
  { key: "status", header: "Status" },
  { key: "source", header: "Source" },
  { key: "reportedDate", header: "Reported Date" },
  { key: "assigned", header: "Assigned" },
  { key: "labels", header: "Labels" },
  { key: "watchers", header: "Watchers" },
  { key: "comments", header: "Comments" },
];

export const IssueListTable: React.FC<IssueListTableProps> = ({
  issues,
  isLoading,
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
    />
  );
};

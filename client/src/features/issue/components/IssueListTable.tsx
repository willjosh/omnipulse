import React from "react";
import { DataTable } from "@/components/ui/Table";

export interface IssueRow {
  id: number;
  title: string;
  vehicleName: string;
  categoryLabel: string;
  priorityLevelLabel: string;
  statusLabel: string;
  reportedByUserName: string;
  reportedDate: string;
  resolvedByUserName?: string;
  resolvedDate?: string;
}

interface IssueListTableProps {
  issues: IssueRow[];
  isLoading?: boolean;
  emptyState?: React.ReactNode;
  onRowClick?: (row: IssueRow) => void;
}

const columns = [
  { key: "title", header: "Title", width: "280px" },
  { key: "vehicleName", header: "Vehicle", width: "180px" },
  { key: "categoryLabel", header: "Category" },
  { key: "priorityLevelLabel", header: "Priority" },
  { key: "statusLabel", header: "Status" },
  { key: "reportedByUserName", header: "Reported By", width: "150px" },
  { key: "reportedDate", header: "Reported Date", width: "190px" },
  { key: "resolvedByUserName", header: "Resolved By", width: "150px" },
  { key: "resolvedDate", header: "Resolved Date", width: "190px" },
];

export const IssueListTable: React.FC<IssueListTableProps> = ({
  issues,
  isLoading,
  emptyState,
  onRowClick,
}) => {
  return (
    <DataTable
      columns={columns}
      data={issues}
      selectedItems={[]}
      onSelectItem={() => {}}
      onSelectAll={() => {}}
      getItemId={item => String(item.id)}
      loading={isLoading}
      showActions={false}
      emptyState={emptyState}
      onRowClick={onRowClick}
      fixedLayout={false}
    />
  );
};

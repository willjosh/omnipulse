import React from "react";
import { DataTable } from "@/components/ui/Table";
import {
  formatEmptyValue,
  formatEmptyValueWithUnknown,
} from "@/utils/emptyValueUtils";

export interface IssueRow {
  id: number;
  title: string;
  vehicleName: string;
  categoryLabel: string;
  priorityLevelLabel: string;
  statusLabel: string;
  reportedByUserName: string | null;
  reportedDate?: string | null;
  resolvedByUserName?: string | null;
  resolvedDate?: string | null;
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
  {
    key: "reportedByUserName",
    header: "Reported By",
    width: "150px",
    render: (item: IssueRow) =>
      formatEmptyValueWithUnknown(item.reportedByUserName),
  },
  {
    key: "reportedDate",
    header: "Reported Date",
    width: "190px",
    render: (item: IssueRow) => {
      if (!item.reportedDate) {
        return <span className="text-gray-400">—</span>;
      }
      const date = new Date(item.reportedDate);
      if (isNaN(date.getTime())) {
        return <span className="text-gray-400">—</span>;
      }
      return (
        <div>
          <div className="font-medium">{date.toLocaleDateString()}</div>
          <div className="text-sm text-gray-500">
            {date.toLocaleTimeString()}
          </div>
        </div>
      );
    },
  },
  {
    key: "resolvedByUserName",
    header: "Resolved By",
    width: "150px",
    render: (item: IssueRow) =>
      formatEmptyValueWithUnknown(item.resolvedByUserName),
  },
  {
    key: "resolvedDate",
    header: "Resolved Date",
    width: "190px",
    render: (item: IssueRow) => {
      if (!item.resolvedDate) {
        return <span className="text-gray-400">—</span>;
      }
      const date = new Date(item.resolvedDate);
      if (isNaN(date.getTime())) {
        return <span className="text-gray-400">—</span>;
      }
      return (
        <div>
          <div className="font-medium">{date.toLocaleDateString()}</div>
          <div className="text-sm text-gray-500">
            {date.toLocaleTimeString()}
          </div>
        </div>
      );
    },
  },
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

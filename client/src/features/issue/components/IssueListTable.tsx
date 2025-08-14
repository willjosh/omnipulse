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
  category: string;
  prioritylevel: string;
  status: string;
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
  onSort?: (sortKey: string) => void;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}

const columns = [
  {
    key: "title",
    header: "Title",
    width: "280px",
    sortable: true, // title - supported
  },
  {
    key: "vehiclename",
    header: "Vehicle",
    width: "180px",
    sortable: false, // Not in backend sortable fields
  },
  {
    key: "category",
    header: "Category",
    sortable: true, // category - supported
  },
  {
    key: "prioritylevel",
    header: "Priority",
    sortable: true, // prioritylevel - supported
  },
  {
    key: "status",
    header: "Status",
    sortable: true, // status - supported
  },
  {
    key: "reportedbyusername",
    header: "Reported By",
    width: "150px",
    sortable: false, // Not in backend sortable fields
    render: (item: IssueRow) =>
      formatEmptyValueWithUnknown(item.reportedByUserName),
  },
  {
    key: "reporteddate",
    header: "Reported Date",
    width: "190px",
    sortable: true, // reporteddate - supported
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
    key: "resolvedbyusername",
    header: "Resolved By",
    width: "150px",
    sortable: false, // Not in backend sortable fields
    render: (item: IssueRow) =>
      formatEmptyValueWithUnknown(item.resolvedByUserName),
  },
  {
    key: "resolveddate",
    header: "Resolved Date",
    width: "190px",
    sortable: true, // resolveddate - supported
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
  onSort,
  sortBy,
  sortOrder,
}) => {
  return (
    <DataTable
      columns={columns}
      data={issues}
      getItemId={item => String(item.id)}
      loading={isLoading}
      showActions={false}
      emptyState={emptyState}
      onRowClick={onRowClick}
      fixedLayout={false}
      onSort={onSort}
      sortBy={sortBy}
      sortOrder={sortOrder}
    />
  );
};

"use client";
import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { Plus } from "lucide-react";
import PrimaryButton from "@/components/ui/Button/PrimaryButton";
import { FilterBar } from "@/components/ui/Filter";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { useIssues } from "@/features/issue/hooks/useIssues";
import { IssueWithLabels } from "@/features/issue/types/issueType";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { formatEmptyValueWithUnknown } from "@/utils/emptyValueUtils";

export default function IssuesPage() {
  const router = useRouter();
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortBy, setSortBy] = useState("reporteddate");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc");

  const filter = useMemo(
    () => ({
      PageNumber: page,
      PageSize: pageSize,
      Search: search,
      SortBy: sortBy,
      SortDescending: sortOrder === "desc",
    }),
    [page, pageSize, search, sortBy, sortOrder],
  );

  const { issues, pagination, isPending } = useIssues(filter);

  const issueTableColumns = [
    {
      key: "title",
      header: "Title",
      width: "280px",
      sortable: true,
      render: (item: IssueWithLabels) => <div>{item.title}</div>,
    },
    {
      key: "vehiclename",
      header: "Vehicle",
      width: "180px",
      sortable: false,
      render: (item: IssueWithLabels) => <div>{item.vehicleName}</div>,
    },
    {
      key: "category",
      header: "Category",
      sortable: true,
      render: (item: IssueWithLabels) => <div>{item.categoryLabel}</div>,
    },
    {
      key: "prioritylevel",
      header: "Priority",
      sortable: true,
      render: (item: IssueWithLabels) => <div>{item.priorityLevelLabel}</div>,
    },
    {
      key: "status",
      header: "Status",
      sortable: true,
      render: (item: IssueWithLabels) => <div>{item.statusLabel}</div>,
    },
    {
      key: "reportedbyusername",
      header: "Reported By",
      width: "150px",
      sortable: false,
      render: (item: IssueWithLabels) =>
        formatEmptyValueWithUnknown(item.reportedByUserName),
    },
    {
      key: "reporteddate",
      header: "Reported Date",
      width: "190px",
      sortable: true,
      render: (item: IssueWithLabels) => {
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
      sortable: false,
      render: (item: IssueWithLabels) =>
        formatEmptyValueWithUnknown(item.resolvedByUserName),
    },
    {
      key: "resolveddate",
      header: "Resolved Date",
      width: "190px",
      sortable: true,
      render: (item: IssueWithLabels) => {
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

  const handlePreviousPage = () => setPage(p => Math.max(1, p - 1));
  const handleNextPage = () =>
    setPage(p => (pagination && p < pagination.totalPages ? p + 1 : p));
  const handlePageChange = (p: number) => setPage(p);
  const handlePageSizeChange = (size: number) => {
    setPageSize(size);
    setPage(1);
  };

  React.useEffect(() => {
    setPage(1);
  }, [search, sortBy, sortOrder]);

  const handleRowClick = (row: IssueWithLabels) => {
    router.push(`/issues/${row.id}`);
  };

  const handleSort = (sortKey: string) => {
    if (sortBy === sortKey) {
      setSortOrder(sortOrder === "asc" ? "desc" : "asc");
    } else {
      setSortBy(sortKey);
      setSortOrder("asc");
    }
    setPage(1);
  };

  const emptyState = (
    <div className="text-center py-8">
      <p className="text-gray-500 mb-2">No issues found.</p>
      <p className="text-gray-400 mb-4">
        Issues are used to track problems or defects reported for vehicles.
      </p>
      <button
        onClick={() => setSearch("")}
        className="text-blue-600 hover:text-blue-800 text-sm"
      >
        Clear search
      </button>
    </div>
  );

  return (
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">Issues</h1>
        <PrimaryButton onClick={() => router.push("/issues/new")}>
          <Plus size={16} />
          Add Issue
        </PrimaryButton>
      </div>
      <div className="flex items-center justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search"
          onFilterChange={() => {}}
        />
        {pagination && (
          <PaginationControls
            currentPage={pagination.pageNumber}
            totalPages={pagination.totalPages}
            totalItems={pagination.totalCount}
            itemsPerPage={pagination.pageSize}
            onPreviousPage={handlePreviousPage}
            onNextPage={handleNextPage}
            onPageChange={handlePageChange}
            onPageSizeChange={handlePageSizeChange}
            className="ml-auto"
          />
        )}
      </div>
      <DataTable
        columns={issueTableColumns}
        data={issues}
        getItemId={item => item.id.toString()}
        loading={isPending}
        showActions={false}
        emptyState={emptyState}
        onRowClick={handleRowClick}
        fixedLayout={false}
        onSort={handleSort}
        sortBy={sortBy}
        sortOrder={sortOrder}
      />
    </div>
  );
}

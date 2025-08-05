"use client";
import React, { useState, useMemo } from "react";
import { useRouter } from "next/navigation";
import { Plus } from "lucide-react";
import PrimaryButton from "@/components/ui/Button/PrimaryButton";
import { FilterBar } from "@/components/ui/Filter";
import { PaginationControls } from "@/components/ui/Table";
import { useIssues } from "@/features/issue/hooks/useIssues";
import { IssueWithLabels } from "@/features/issue/types/issueType";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import {
  IssueListTable,
  IssueRow,
} from "@/features/issue/components/IssueListTable";

export default function IssuesPage() {
  const router = useRouter();
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);

  const filter = useMemo(
    () => ({ PageNumber: page, PageSize: pageSize, Search: search }),
    [page, pageSize, search],
  );

  const { issues, pagination, isPending } = useIssues(filter);

  const tableData: IssueRow[] = useMemo(
    () =>
      issues.map((issue: IssueWithLabels) => ({
        id: issue.id,
        issueNumber: issue.issueNumber,
        title: issue.title,
        vehicleName: issue.vehicleName,
        categoryLabel: issue.categoryLabel,
        priorityLevelLabel: issue.priorityLevelLabel,
        statusLabel: issue.statusLabel,
        reportedByUserName: issue.reportedByUserName,
        reportedDate: issue.reportedDate || "Unknown",
        resolvedByUserName: issue.resolvedByUserName || "Unassigned",
        resolvedDate: issue.resolvedDate || "",
      })),
    [issues],
  );

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
  }, [search]);

  const handleRowClick = (row: IssueRow) => {
    router.push(`/issues/${row.id}`);
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
    <div className="p-6 w-full max-w-none min-h-screen">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">Issues</h1>
        <PrimaryButton onClick={() => router.push("/issues/new")}>
          <Plus className="w-5 h-5" />
          <span className="flex items-center">Add Issue</span>
        </PrimaryButton>
      </div>
      <div className="flex items-center justify-between mb-6">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search issues"
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
      <IssueListTable
        issues={tableData}
        isLoading={isPending}
        emptyState={emptyState}
        onRowClick={handleRowClick}
      />
    </div>
  );
}

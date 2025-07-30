"use client";
import React, { useState, useMemo } from "react";
import { IssueListFilters } from "../../features/issue/config/IssueListFilters";
import {
  IssueListTable,
  IssueRow,
} from "../../features/issue/components/IssueListTable";
import { PaginationControls } from "../../components/ui/Table";
import { useIssues } from "@/features/issue/hooks/useIssues";
import { useRouter } from "next/navigation";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { IssueWithLabels } from "@/features/issue/types/issueType";

export default function IssuesPage() {
  // State for filters and pagination
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);

  // Compose filter object for useIssues
  const filter = useMemo(
    () => ({ PageNumber: page, PageSize: pageSize, Search: search }),
    [page, pageSize, search],
  );

  const { issues, pagination, isPending } = useIssues(filter);

  // Map issues to IssueRow for the table
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

  // Pagination handlers
  const handlePreviousPage = () => setPage(p => Math.max(1, p - 1));
  const handleNextPage = () =>
    setPage(p => (pagination && p < pagination.totalPages ? p + 1 : p));
  const handlePageChange = (p: number) => setPage(p);
  const handlePageSizeChange = (size: number) => {
    setPageSize(size);
    setPage(1);
  };

  // Reset page when search changes
  React.useEffect(() => {
    setPage(1);
  }, [search]);

  const router = useRouter();

  // Handler for row click to navigate to details page
  const handleRowClick = (row: IssueRow) => {
    router.push(`/issues/${row.id}`);
  };

  return (
    <div className="container mx-auto px-4 mt-4">
      <div className="flex items-center justify-between mb-2">
        <h1 className="text-2xl font-bold">Issues</h1>
        <button
          className="bg-blue-600 text-white px-4 rounded-lg font-semibold hover:bg-blue-700 transition"
          onClick={() => router.push("/issues/new")}
        >
          Add Issue
        </button>
      </div>
      {/* Inline filters and pagination controls */}
      <div className="flex flex-wrap items-center gap-4 mb-4 justify-between">
        <IssueListFilters searchValue={search} onSearchChange={setSearch} />
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
      <div className="bg-white rounded-lg shadow">
        <IssueListTable
          issues={tableData}
          isLoading={isPending}
          emptyState={
            <div>
              <div className="text-lg font-semibold text-gray-500 mb-2">
                No issues found.
              </div>
              <div className="text-gray-400">
                Try adjusting your filters or search.
              </div>
            </div>
          }
          onRowClick={handleRowClick}
        />
      </div>
    </div>
  );
}

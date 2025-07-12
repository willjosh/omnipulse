"use client";
import React, { useState, useMemo } from "react";
import { IssueTabs } from "../_features/issue/components/IssueTabs";
import { IssueListFilters } from "../_features/issue/components/IssueListFilters";
import {
  IssueListTable,
  IssueRow,
} from "../_features/issue/components/IssueListTable";
import { PaginationControls } from "../_features/shared/table";
import { useIssues } from "../_hooks/issue/useIssues";
import { useRouter } from "next/navigation";
import { DEFAULT_PAGE_SIZE } from "@/app/_hooks/issue/useIssues";
import { IssueWithLabels } from "../_hooks/issue/issueType";

export default function IssuesPage() {
  // State for filters and pagination
  const [activeTab, setActiveTab] = useState<string>("all");
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const [priority, setPriority] = useState("");
  const [category, setCategory] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);

  // Compose filter object for useIssues
  const filter = useMemo(
    () => ({ page, pageSize, search }),
    [page, pageSize, search],
  );

  const { issues, pagination, isPending } = useIssues(filter);

  // Map issues to IssueRow for the table
  const tableData: IssueRow[] = useMemo(
    () =>
      issues.map((issue: IssueWithLabels) => ({
        id: issue.id.toString(),
        priority: issue.PriorityLevelLabel,
        assetName: issue.VehicleName,
        assetRecordType: "Vehicle",
        number: `#${issue.IssueNumber}`,
        summary: issue.Title,
        status: issue.StatusLabel,
        source: issue.ReportedByUserName,
        reportedDate: issue.ReportedDate || "Unknown",
        assigned: issue.ResolvedByUserName || "Unknown",
        labels: issue.CategoryLabel,
      })),
    [issues],
  );

  // Tab counts (optional, can be fetched separately if needed)
  const tabCounts = undefined;

  // Pagination handlers
  const handlePreviousPage = () => setPage(p => Math.max(1, p - 1));
  const handleNextPage = () =>
    setPage(p => (pagination && p < pagination.totalPages ? p + 1 : p));
  const handlePageChange = (p: number) => setPage(p);
  const handlePageSizeChange = (size: number) => {
    setPageSize(size);
    setPage(1);
  };

  // Reset page when filters change
  React.useEffect(() => {
    setPage(1);
  }, [search, status, priority, category, activeTab]);

  const router = useRouter();

  // Handler for row click to navigate to details page
  const handleRowClick = (row: IssueRow) => {
    router.push(`/issues/${row.id}`);
  };

  return (
    <div className="container mx-auto px-4">
      <div className="flex items-center justify-between mb-2">
        <h1 className="text-2xl font-bold">Issues</h1>
        <button className="bg-blue-600 text-white px-4 rounded-lg font-semibold hover:bg-blue-700 transition">
          Add Issue
        </button>
      </div>
      <IssueTabs
        activeTab={activeTab}
        onTabChange={setActiveTab}
        tabCounts={tabCounts}
      />
      {/* Inline filters and pagination controls */}
      <div className="flex flex-wrap items-center gap-4 mb-4 justify-between">
        <IssueListFilters
          searchValue={search}
          onSearchChange={setSearch}
          status={status}
          onStatusChange={setStatus}
          priority={priority}
          onPriorityChange={setPriority}
          category={category}
          onCategoryChange={setCategory}
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

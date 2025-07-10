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
import { IssueStatusEnum } from "../_hooks/issue/issueEnum";

const DEFAULT_PAGE_SIZE = 10;

const tabToStatus: Record<string, number | undefined> = {
  all: undefined,
  open: IssueStatusEnum.OPEN,
  overdue: IssueStatusEnum.OPEN, // Overdue logic can be refined if needed
  resolved: IssueStatusEnum.RESOLVED,
  closed: IssueStatusEnum.CLOSED,
};

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
  const filter = useMemo(() => {
    let statusValue: number | undefined = undefined;
    if (status) {
      const parsed = Number(status);
      statusValue = isNaN(parsed) ? undefined : parsed;
    } else if (activeTab !== "all") {
      statusValue = tabToStatus[activeTab];
    }
    return {
      page,
      limit: pageSize,
      search,
      status: statusValue,
      priorityLevel: priority ? Number(priority) : undefined,
      category: category ? Number(category) : undefined,
    };
  }, [page, pageSize, search, status, priority, category, activeTab]);

  const { issues, pagination, isPending } = useIssues(filter);

  // Map issues to IssueRow for the table
  const tableData: IssueRow[] = useMemo(
    () =>
      issues.map(issue => ({
        id: issue.id.toString(),
        priority: issue.PriorityLevelLabel,
        assetName: issue.VehicleName,
        assetRecordType: "Vehicle",
        number: `#${issue.IssueNumber}`,
        summary: issue.Title,
        status: issue.StatusLabel,
        source: issue.ReportedByUserName,
        reportedDate: issue.ReportedDate
          ? new Date(issue.ReportedDate).toLocaleDateString()
          : "",
        assigned: issue.ResolvedByUserName || "",
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

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold">Issues</h1>
        <button className="bg-blue-600 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition">
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
        />
      </div>
    </div>
  );
}

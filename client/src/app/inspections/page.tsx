"use client";
import React, { useState, useMemo, useEffect } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { FilterBar } from "@/components/ui/Filter";
import { PrimaryButton } from "@/components/ui/Button";
import { useInspections } from "@/features/inspection/hooks/useInspections";
import { useInspectionForms } from "@/features/inspection-form/hooks/useInspectionForms";
import { InspectionWithLabels } from "@/features/inspection/types/inspectionType";
import { InspectionForm } from "@/features/inspection-form/types/inspectionFormType";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { AlertTriangle, CheckCircle, XCircle, Plus } from "lucide-react";
import ModalPortal from "@/components/ui/Modal/ModalPortal";

export default function InspectionListPage() {
  const router = useRouter();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [showSelectFormModal, setShowSelectFormModal] = useState(false);

  useEffect(() => {
    setPage(1);
  }, [search]);

  const filter = useMemo(
    () => ({ PageNumber: page, PageSize: pageSize, Search: search }),
    [page, pageSize, search],
  );

  const { inspections, pagination, isPending } = useInspections(filter);
  const { inspectionForms } = useInspectionForms({ PageSize: 100 });

  const columns = [
    {
      key: "vehicleName",
      header: "Vehicle",
      sortable: true,
      width: "150px",
      render: (item: InspectionWithLabels) => (
        <div>
          <div className="font-medium">{item.vehicleName}</div>
        </div>
      ),
    },
    {
      key: "inspectionFormName",
      header: "Form Type",
      sortable: true,
      width: "180px",
      render: (item: InspectionWithLabels) => (
        <div>
          <div className="font-medium">{item.inspectionFormName}</div>
          <div className="text-sm text-gray-500">{item.snapshotFormTitle}</div>
        </div>
      ),
    },
    {
      key: "technicianName",
      header: "Inspector",
      sortable: true,
      width: "150px",
      render: (item: InspectionWithLabels) => (
        <div>
          <div className="font-medium">{item.technicianName}</div>
        </div>
      ),
    },
    {
      key: "inspectionEndTime",
      header: "Submission Time",
      sortable: true,
      width: "160px",
      render: (item: InspectionWithLabels) => (
        <div>
          <div className="font-medium">
            {new Date(item.inspectionEndTime).toLocaleDateString()}
          </div>
          <div className="text-sm text-gray-500">
            {new Date(item.inspectionEndTime).toLocaleTimeString()}
          </div>
        </div>
      ),
    },
    {
      key: "vehicleCondition",
      header: "Condition",
      sortable: true,
      width: "140px",
      render: (item: InspectionWithLabels) => (
        <div className="flex items-center gap-2">
          <span
            className={`px-2 py-1 rounded-full text-xs font-medium ${
              item.vehicleConditionEnum === 1
                ? "bg-green-100 text-green-800"
                : item.vehicleConditionEnum === 2
                  ? "bg-yellow-100 text-yellow-800"
                  : "bg-red-100 text-red-800"
            }`}
          >
            {item.vehicleConditionLabel}
          </span>
        </div>
      ),
    },
    {
      key: "inspectionResults",
      header: "Results",
      width: "120px",
      render: (item: InspectionWithLabels) => (
        <div className="flex items-center gap-2">
          <div className="flex items-center gap-1">
            <CheckCircle size={14} className="text-green-600" />
            <span className="text-sm">{item.passedItemsCount}</span>
          </div>
          <div className="flex items-center gap-1">
            <XCircle size={14} className="text-red-600" />
            <span className="text-sm">{item.failedItemsCount}</span>
          </div>
        </div>
      ),
    },
    {
      key: "failedItemsIndicator",
      header: "Failed Items",
      width: "100px",
      render: (item: InspectionWithLabels) => (
        <div className="flex items-center justify-center">
          {item.failedItemsCount > 0 ? (
            <div className="flex items-center gap-1 text-red-600">
              <AlertTriangle size={16} />
              <span className="text-sm font-medium">
                {item.failedItemsCount}
              </span>
            </div>
          ) : (
            <div className="flex items-center gap-1 text-green-600">
              <CheckCircle size={16} />
              <span className="text-sm font-medium">Pass</span>
            </div>
          )}
        </div>
      ),
    },
    {
      key: "odometerReading",
      header: "Odometer",
      width: "100px",
      render: (item: InspectionWithLabels) =>
        item.odometerReading ? (
          <span className="text-sm">
            {item.odometerReading.toLocaleString()} km
          </span>
        ) : (
          <span className="text-sm text-gray-400">-</span>
        ),
    },
  ];

  const handleRowClick = (row: InspectionWithLabels) => {
    router.push(`/inspections/${row.id}`);
  };

  const emptyState = (
    <div className="text-center py-8">
      <p className="text-gray-500 mb-2">No inspections found.</p>
      <button
        onClick={() => setSearch("")}
        className="text-blue-600 hover:text-blue-800 text-sm"
      >
        Clear search
      </button>
    </div>
  );

  const handleFormSelect = (form: InspectionForm) => {
    setShowSelectFormModal(false);
    router.push(`/inspections/new?formId=${form.id}`);
  };

  return (
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">
          Inspection History
        </h1>
        <PrimaryButton onClick={() => setShowSelectFormModal(true)}>
          <Plus size={16} />
          Start Inspection
        </PrimaryButton>
      </div>
      <div className="flex items-end justify-between mb-4">
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
            onPreviousPage={() => setPage(p => Math.max(1, p - 1))}
            onNextPage={() =>
              setPage(p =>
                pagination && p < pagination.totalPages ? p + 1 : p,
              )
            }
            onPageChange={setPage}
            onPageSizeChange={setPageSize}
          />
        )}
      </div>
      <DataTable
        data={inspections}
        columns={columns}
        selectedItems={[]}
        onSelectItem={() => {}}
        onSelectAll={() => {}}
        onRowClick={handleRowClick}
        loading={isPending}
        emptyState={emptyState}
        getItemId={item => item.id.toString()}
        showActions={false}
        fixedLayout={false}
      />

      {/* Select Inspection Form Modal */}
      <ModalPortal isOpen={showSelectFormModal}>
        <div className="fixed inset-0 backdrop-brightness-50 bg-opacity-50 flex items-center justify-center z-[100]">
          <div className="bg-white rounded-lg shadow-xl max-w-md w-full mx-4">
            {/* Header */}
            <div className="px-6 py-4 border-b border-gray-200">
              <div className="flex items-center justify-between">
                <h2 className="text-lg font-semibold text-gray-900">
                  Select Inspection Form
                </h2>
                <button
                  onClick={() => setShowSelectFormModal(false)}
                  className="text-gray-400 hover:text-gray-600"
                >
                  <svg
                    className="w-6 h-6"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth="2"
                      d="M6 18L18 6M6 6l12 12"
                    />
                  </svg>
                </button>
              </div>
            </div>

            {/* Content */}
            <div className="px-6 py-4">
              <div className="mb-4">
                <h3 className="text-sm font-medium text-gray-700 mb-2">ALL</h3>
                <div className="space-y-2">
                  {inspectionForms
                    .filter(form => form.isActive)
                    .map(form => (
                      <div
                        key={form.id}
                        onClick={() => handleFormSelect(form)}
                        className="p-3 border border-gray-200 rounded-lg hover:bg-gray-50 cursor-pointer transition-colors"
                      >
                        <div className="font-medium text-gray-900">
                          {form.title}
                        </div>
                        {form.description && (
                          <div className="text-sm text-gray-500 mt-1">
                            {form.description}
                          </div>
                        )}
                      </div>
                    ))}
                </div>
              </div>
            </div>

            {/* Footer */}
            <div className="px-6 py-4 border-t border-gray-200 bg-gray-50 rounded-b-lg">
              <div className="flex justify-between">
                <button
                  onClick={() => router.push("/inspection-forms/new")}
                  className="text-blue-600 hover:text-blue-700 font-medium"
                >
                  Add Inspection Form
                </button>
                <button
                  onClick={() => setShowSelectFormModal(false)}
                  className="text-blue-600 hover:text-blue-700 font-medium"
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      </ModalPortal>
    </div>
  );
}

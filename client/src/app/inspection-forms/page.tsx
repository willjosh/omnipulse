"use client";
import React, { useState, useMemo, useEffect } from "react";
import { useRouter } from "next/navigation";
import { DataTable, PaginationControls } from "@/components/ui/Table";
import { FilterBar } from "@/components/ui/Filter";
import { PrimaryButton } from "@/components/ui/Button";
import {
  useInspectionForms,
  useDeactivateInspectionForm,
} from "@/features/inspection-form/hooks/useInspectionForms";
import { InspectionForm } from "@/features/inspection-form/types/inspectionFormType";
import { DEFAULT_PAGE_SIZE } from "@/components/ui/Table/constants";
import { Plus, Trash2, CheckCircle, XCircle } from "lucide-react";
import { ConfirmModal } from "@/components/ui/Modal";
import { useNotification } from "@/components/ui/Feedback/NotificationProvider";
import ModalPortal from "@/components/ui/Modal/ModalPortal";

export default function InspectionFormsPage() {
  const router = useRouter();
  const notify = useNotification();
  const { mutate: deactivateInspectionForm, isPending: isDeactivating } =
    useDeactivateInspectionForm();

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);

  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    inspectionForm: InspectionForm | null;
  }>({ isOpen: false, inspectionForm: null });

  const [showSelectFormModal, setShowSelectFormModal] = useState(false);

  useEffect(() => {
    setPage(1);
  }, [search]);

  const filter = useMemo(
    () => ({ PageNumber: page, PageSize: pageSize, Search: search }),
    [page, pageSize, search],
  );

  const { inspectionForms, pagination, isPending } = useInspectionForms(filter);

  const columns = [
    {
      key: "title",
      header: "Form Name",
      sortable: true,
      width: "250px",
      render: (item: InspectionForm) => (
        <div>
          <div className="font-medium text-gray-900">{item.title}</div>
          {item.description && (
            <div className="text-sm text-gray-500 mt-1">{item.description}</div>
          )}
        </div>
      ),
    },
    {
      key: "inspectionFormItemCount",
      header: "Checklist Items",
      sortable: true,
      width: "140px",
      render: (item: InspectionForm) => (
        <div className="text-center">
          <span className="text-sm font-medium text-gray-900">
            {item.inspectionFormItemCount}
          </span>
        </div>
      ),
    },
    {
      key: "inspectionCount",
      header: "Submissions",
      sortable: true,
      width: "120px",
      render: (item: InspectionForm) => (
        <div className="text-center">
          <span className="text-sm font-medium text-gray-900">
            {item.inspectionCount}
          </span>
        </div>
      ),
    },
    {
      key: "updatedAt",
      header: "Updated",
      sortable: true,
      width: "120px",
      render: (item: InspectionForm) =>
        new Date(item.updatedAt).toLocaleDateString(),
    },
  ];

  const handleRowClick = (row: InspectionForm) => {
    router.push(`/inspection-forms/${row.id}`);
  };

  const handleDeactivateInspectionForm = async () => {
    if (!confirmModal.inspectionForm) return;

    deactivateInspectionForm(confirmModal.inspectionForm.id, {
      onSuccess: () => {
        notify("Inspection form deactivated successfully!", "success");
        setConfirmModal({ isOpen: false, inspectionForm: null });
      },
      onError: error => {
        console.error("Failed to deactivate inspection form:", error);
        notify("Failed to deactivate inspection form", "error");
        setConfirmModal({ isOpen: false, inspectionForm: null });
      },
    });
  };

  const inspectionFormActions = useMemo(
    () => [
      {
        key: "deactivate",
        label: "Deactivate",
        icon: <Trash2 size={16} />,
        variant: "danger" as const,
        onClick: (inspectionForm: InspectionForm) => {
          setConfirmModal({ isOpen: true, inspectionForm });
        },
        show: (inspectionForm: InspectionForm) => inspectionForm.isActive,
      },
    ],
    [],
  );

  const emptyState = (
    <div className="text-center py-8">
      <p className="text-gray-500 mb-2">No inspection forms found.</p>
      <button
        onClick={() => setSearch("")}
        className="text-blue-600 hover:text-blue-800 text-sm"
      >
        Clear search
      </button>
    </div>
  );

  const [selectedItems, setSelectedItems] = useState<string[]>([]);
  const handleSelectItem = (id: string) => {
    setSelectedItems(items =>
      items.includes(id)
        ? items.filter(itemId => itemId !== id)
        : [...items, id],
    );
  };
  const handleSelectAll = () => {
    if (inspectionForms.length === 0) return;
    if (selectedItems.length === inspectionForms.length) {
      setSelectedItems([]);
    } else {
      setSelectedItems(inspectionForms.map(item => item.id.toString()));
    }
  };

  const handleFormSelect = (form: InspectionForm) => {
    setShowSelectFormModal(false);
    router.push(`/inspections/new?formId=${form.id}`);
  };

  return (
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">
          Inspection Forms
        </h1>
        <div className="flex gap-3">
          <PrimaryButton onClick={() => setShowSelectFormModal(true)}>
            <Plus size={18} />
            Start Inspection
          </PrimaryButton>
          <PrimaryButton onClick={() => router.push("/inspection-forms/new")}>
            <Plus size={18} />
            Add Inspection Form
          </PrimaryButton>
        </div>
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
        data={inspectionForms}
        columns={columns}
        selectedItems={selectedItems}
        onSelectItem={handleSelectItem}
        onSelectAll={handleSelectAll}
        onRowClick={handleRowClick}
        loading={isPending}
        emptyState={emptyState}
        getItemId={item => item.id.toString()}
        showActions={true}
        actions={inspectionFormActions}
        fixedLayout={false}
      />

      <ConfirmModal
        isOpen={confirmModal.isOpen}
        onClose={() =>
          !isDeactivating &&
          setConfirmModal({ isOpen: false, inspectionForm: null })
        }
        onConfirm={handleDeactivateInspectionForm}
        title="Deactivate Inspection Form"
        message={
          confirmModal.inspectionForm
            ? `Are you sure you want to deactivate "${confirmModal.inspectionForm.title}"? This will make the form unavailable for new inspections but preserve existing inspection history.`
            : ""
        }
        confirmText={isDeactivating ? "Deactivating..." : "Deactivate"}
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

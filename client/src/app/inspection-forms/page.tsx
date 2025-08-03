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
      key: "isActive",
      header: "Status",
      sortable: true,
      width: "100px",
      render: (item: InspectionForm) => (
        <div className="flex items-center gap-2">
          {item.isActive ? (
            <div className="flex items-center gap-1 text-green-600">
              <CheckCircle size={16} />
              <span className="text-sm font-medium">Active</span>
            </div>
          ) : (
            <div className="flex items-center gap-1 text-red-600">
              <XCircle size={16} />
              <span className="text-sm font-medium">Inactive</span>
            </div>
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
          <div className="text-xs text-gray-500">items</div>
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
          <div className="text-xs text-gray-500">inspections</div>
        </div>
      ),
    },
    {
      key: "createdAt",
      header: "Created",
      sortable: true,
      width: "120px",
      render: (item: InspectionForm) =>
        new Date(item.createdAt).toLocaleDateString(),
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

  return (
    <div className="p-6 w-full max-w-none">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold text-gray-900">
          Inspection Forms
        </h1>
        <PrimaryButton onClick={() => router.push("/inspection-forms/new")}>
          <Plus size={18} className="mr-2" />
          Add Inspection Form
        </PrimaryButton>
      </div>
      <div className="flex items-end justify-between mb-4">
        <FilterBar
          searchValue={search}
          onSearchChange={setSearch}
          searchPlaceholder="Search inspection forms..."
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
    </div>
  );
}

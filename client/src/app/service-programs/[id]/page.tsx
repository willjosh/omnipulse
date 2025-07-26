"use client";
import React, { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import ServiceProgramHeader from "@/app/_features/service-program/components/ServiceProgramHeader";
import { TabNavigation } from "@/app/_features/shared/tabs";
import { PrimaryButton } from "@/app/_features/shared/button";
import { ChevronDown, Plus } from "lucide-react";
import { useServiceProgram } from "@/app/_hooks/service-program/useServicePrograms";
import { useServiceSchedules } from "@/app/_hooks/service-schedule/useServiceSchedules";
import Loading from "@/app/_features/shared/feedback/Loading";
import EditIcon from "@/app/_features/shared/icons/Edit";
import EmptyState from "@/app/_features/shared/feedback/EmptyState";
import DataTable from "@/app/_features/shared/table/DataTable";
import FilterBar from "@/app/_features/shared/filter/FilterBar";
import PaginationControls from "@/app/_features/shared/table/PaginationControls";
import { ServiceScheduleWithLabels } from "@/app/_hooks/service-schedule/serviceScheduleType";

export default function ServiceProgramDetailsPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id ? Number(params.id) : undefined;

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const {
    data: serviceProgram,
    isPending: isLoadingProgram,
    isError: isProgramError,
  } = useServiceProgram(id!);
  const {
    serviceSchedules,
    pagination,
    isPending: isLoadingSchedules,
  } = useServiceSchedules({
    PageNumber: page,
    PageSize: pageSize,
    Search: search,
  });

  const [activeTab, setActiveTab] = useState("schedules");
  const [isAddDropdownOpen, setIsAddDropdownOpen] = useState(false);
  const [selectedItems, setSelectedItems] = useState<string[]>([]);

  const tabs = [
    { key: "schedules", label: "Service Schedules" },
    { key: "vehicles", label: "Vehicles" },
  ];

  // Reset page to 1 when search changes
  useEffect(() => {
    setPage(1);
  }, [search]);

  if (isLoadingProgram) {
    return <Loading />;
  }

  if (isProgramError || !serviceProgram) {
    return (
      <EmptyState
        title="Service Program not found"
        message="The service program you are looking for does not exist or could not be loaded."
      />
    );
  }

  const breadcrumbs = [
    { label: "Service Programs", href: "/service-programs" },
  ];

  const handleEdit = () => {
    router.push(`/service-programs/${id}/edit`);
  };

  const handleAddServiceSchedule = () => {
    // TODO: Navigate to add service schedule page
    console.log("Add service schedule");
    setIsAddDropdownOpen(false);
  };

  const handleAddVehicle = () => {
    // TODO: Navigate to add vehicle page
    console.log("Add vehicle");
    setIsAddDropdownOpen(false);
  };

  const handleSelectItem = (itemId: string) => {
    setSelectedItems(prev =>
      prev.includes(itemId)
        ? prev.filter(id => id !== itemId)
        : [...prev, itemId],
    );
  };

  const handleSelectAll = () => {
    if (selectedItems.length === serviceSchedules.length) {
      setSelectedItems([]);
    } else {
      setSelectedItems(
        serviceSchedules.map(schedule => schedule.id.toString()),
      );
    }
  };

  const handleRowClick = (schedule: ServiceScheduleWithLabels) => {
    router.push(`/service-schedules/${schedule.id}`);
  };

  const handlePageChange = (newPage: number) => {
    setPage(newPage);
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize);
    setPage(1); // Reset to first page when changing page size
  };

  const serviceScheduleColumns = [
    { key: "name", header: "Name", width: "200px" },
    {
      key: "frequency",
      header: "Frequency",
      width: "150px",
      render: (schedule: ServiceScheduleWithLabels) => {
        if (schedule.timeIntervalValue && schedule.timeIntervalUnitLabel) {
          return `${schedule.timeIntervalValue} ${
            schedule.timeIntervalValue === 1
              ? schedule.timeIntervalUnitLabel.replace(/s$/, "")
              : schedule.timeIntervalUnitLabel
          }`;
        } else if (schedule.mileageInterval) {
          return `${schedule.mileageInterval} km`;
        }
        return "-";
      },
    },
    {
      key: "buffer",
      header: "Buffer",
      width: "150px",
      render: (schedule: ServiceScheduleWithLabels) => {
        if (schedule.timeBufferValue && schedule.timeBufferUnitLabel) {
          return `${schedule.timeBufferValue} ${
            schedule.timeBufferValue === 1
              ? schedule.timeBufferUnitLabel.replace(/s$/, "")
              : schedule.timeBufferUnitLabel
          }`;
        } else if (schedule.mileageBuffer) {
          return `${schedule.mileageBuffer} km`;
        }
        return "-";
      },
    },
    {
      key: "firstService",
      header: "First Service",
      width: "150px",
      render: (schedule: ServiceScheduleWithLabels) => {
        if (
          schedule.firstServiceTimeValue &&
          schedule.firstServiceTimeUnitLabel
        ) {
          return `${schedule.firstServiceTimeValue} ${
            schedule.firstServiceTimeValue === 1
              ? schedule.firstServiceTimeUnitLabel.replace(/s$/, "")
              : schedule.firstServiceTimeUnitLabel
          }`;
        } else if (schedule.firstServiceMileage) {
          return `${schedule.firstServiceMileage} km`;
        }
        return "-";
      },
    },
    {
      key: "isActive",
      header: "Active",
      width: "100px",
      render: (schedule: ServiceScheduleWithLabels) =>
        schedule.isActive ? "Yes" : "No",
    },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      <ServiceProgramHeader
        title={serviceProgram.name}
        breadcrumbs={breadcrumbs}
        actions={
          <>
            <PrimaryButton onClick={handleEdit}>
              <EditIcon /> Edit
            </PrimaryButton>
            <div className="relative ml-2">
              <PrimaryButton
                onClick={() => setIsAddDropdownOpen(!isAddDropdownOpen)}
                className="flex items-center"
              >
                <Plus size={14} />
                Add
                <ChevronDown size={16} />
              </PrimaryButton>
              {isAddDropdownOpen && (
                <>
                  <div className="absolute right-0 mt-1 w-48 bg-white rounded-md shadow-lg border border-gray-200 z-20">
                    <div className="py-1">
                      <button
                        onClick={handleAddServiceSchedule}
                        className="flex items-center w-full text-left px-3 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors"
                      >
                        Add Service Schedule
                      </button>
                      <button
                        onClick={handleAddVehicle}
                        className="flex items-center w-full text-left px-3 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors"
                      >
                        Add Vehicles
                      </button>
                    </div>
                  </div>
                  <div
                    className="fixed inset-0 z-10"
                    onClick={() => setIsAddDropdownOpen(false)}
                  />
                </>
              )}
            </div>
          </>
        }
      />
      <div className="px-6 mt-4">
        {/* Service Program Description */}
        {serviceProgram.description &&
          serviceProgram.description.trim() !== "" && (
            <div className="mb-6 p-4 bg-white rounded-lg shadow">
              <h3 className="text-sm font-medium text-gray-900 mb-2">
                Description
              </h3>
              <p className="text-sm text-gray-600">
                {serviceProgram.description}
              </p>
            </div>
          )}

        <TabNavigation
          tabs={tabs}
          activeTab={activeTab}
          onTabChange={setActiveTab}
        />

        {activeTab === "schedules" && (
          <>
            <div className="mb-4 flex items-center justify-between">
              <FilterBar
                searchValue={search}
                onSearchChange={setSearch}
                searchPlaceholder="Search..."
                onFilterChange={() => {}}
              />
              {pagination && (
                <PaginationControls
                  currentPage={pagination.pageNumber}
                  totalPages={pagination.totalPages}
                  totalItems={pagination.totalCount}
                  itemsPerPage={pagination.pageSize}
                  onPreviousPage={() => handlePageChange(page - 1)}
                  onNextPage={() => handlePageChange(page + 1)}
                  onPageChange={handlePageChange}
                  onPageSizeChange={handlePageSizeChange}
                  className="ml-4"
                />
              )}
            </div>

            {!isLoadingSchedules && serviceSchedules.length === 0 ? (
              <div className="bg-white rounded-lg shadow w-full">
                <EmptyState
                  title="No Service Schedules"
                  message={
                    search
                      ? "No service schedules found matching your search criteria."
                      : "No service schedules have been assigned to this service program yet."
                  }
                />
              </div>
            ) : (
              <div className="bg-white rounded-lg shadow w-full">
                <div className="w-full">
                  <DataTable
                    data={serviceSchedules}
                    columns={serviceScheduleColumns}
                    selectedItems={selectedItems}
                    onSelectItem={handleSelectItem}
                    onSelectAll={handleSelectAll}
                    onRowClick={handleRowClick}
                    loading={isLoadingSchedules}
                    emptyState="No service schedules found"
                    getItemId={(item: ServiceScheduleWithLabels) =>
                      item.id.toString()
                    }
                    showActions={false}
                    fixedLayout={false}
                  />
                </div>
              </div>
            )}
          </>
        )}

        {activeTab === "vehicles" && (
          <div className="bg-white rounded-lg shadow w-full">
            <div className="p-8 text-center text-gray-500">
              Vehicles table coming soon...
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

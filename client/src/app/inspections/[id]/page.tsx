"use client";
import React from "react";
import { useParams } from "next/navigation";
import { Printer } from "lucide-react";
import InspectionHeader from "@/features/inspection/components/InspectionHeader";
import { useInspection } from "@/features/inspection/hooks/useInspections";
import SecondaryButton from "@/components/ui/Button/SecondaryButton";
import FormContainer from "@/components/ui/Form/FormContainer";
import DetailFieldRow from "@/components/ui/Detail/DetailFieldRow";
import { format } from "date-fns";
import { formatEmptyValueWithUnknown } from "@/utils/emptyValueUtils";

export default function InspectionDetailsPage() {
  const params = useParams();
  const inspectionId = Number(params.id);

  const { inspection, isPending, isError } = useInspection(inspectionId);

  const breadcrumbs = [{ label: "Inspection History", href: "/inspections" }];

  const handlePrint = () => {
    window.print();
  };

  const formatDateTime = (dateString: string) => {
    try {
      return format(new Date(dateString), "EEE, MMM dd, yyyy h:mma");
    } catch {
      return dateString;
    }
  };

  const calculateDuration = (startTime: string, endTime: string) => {
    try {
      const start = new Date(startTime);
      const end = new Date(endTime);
      const diffMs = end.getTime() - start.getTime();
      const diffSeconds = Math.floor(diffMs / 1000);
      return `${diffSeconds}s`;
    } catch {
      return "—";
    }
  };

  if (isPending) {
    return (
      <div className="min-h-screen bg-gray-50">
        <InspectionHeader title="Loading..." breadcrumbs={breadcrumbs} />
        <div className="flex items-center justify-center h-64">
          <div className="text-gray-500">Loading inspection details...</div>
        </div>
      </div>
    );
  }

  if (isError || !inspection) {
    return (
      <div className="min-h-screen bg-gray-50">
        <InspectionHeader title="Error" breadcrumbs={breadcrumbs} />
        <div className="flex items-center justify-center h-64">
          <div className="text-red-500">Failed to load inspection details</div>
        </div>
      </div>
    );
  }

  const actions = (
    <>
      <SecondaryButton
        onClick={handlePrint}
        className="flex items-center gap-2"
      >
        <Printer className="w-4 h-4" />
        Print
      </SecondaryButton>
    </>
  );

  return (
    <div className="min-h-screen bg-gray-50">
      <InspectionHeader
        title={`Submission #${inspection.id}`}
        breadcrumbs={breadcrumbs}
        actions={actions}
      />

      <div className="m-4 px-6 pb-12">
        {/* Summary Cards */}
        <div className="mb-6 grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="bg-white p-4 rounded-lg border border-gray-200 text-center">
            <div className="text-2xl font-bold text-gray-900">
              {inspection.inspectionItems.length}
            </div>
            <div className="text-sm text-gray-600">Total Items</div>
          </div>
          <div className="bg-white p-4 rounded-lg border border-gray-200 text-center">
            <div className="text-2xl font-bold text-green-600">
              {inspection.inspectionItems.filter(item => item.passed).length}
            </div>
            <div className="text-sm text-gray-600">Passed</div>
          </div>
          <div className="bg-white p-4 rounded-lg border border-gray-200 text-center">
            <div className="text-2xl font-bold text-red-600">
              {inspection.inspectionItems.filter(item => !item.passed).length}
            </div>
            <div className="text-sm text-gray-600">Failed</div>
          </div>
          <div className="bg-white p-4 rounded-lg border border-gray-200 text-center">
            <div className="text-2xl font-bold text-blue-600">
              {inspection.inspectionItems.length > 0
                ? `${Math.round(
                    (inspection.inspectionItems.filter(item => item.passed)
                      .length /
                      inspection.inspectionItems.length) *
                      100,
                  )}%`
                : "0%"}
            </div>
            <div className="text-sm text-gray-600">Pass Rate</div>
          </div>
        </div>

        <div className="flex gap-6">
          {/* Left Column - Inspection Details */}
          <div className="flex-1">
            <FormContainer
              title="Inspection Details"
              className="max-w-2xl mx-auto"
            >
              <DetailFieldRow
                label="Vehicle"
                value={
                  <div className="flex items-center gap-2">
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                      CAR
                    </span>
                    <a
                      href={`/vehicles/${inspection.vehicleID}`}
                      className="text-blue-600 hover:text-blue-800 underline"
                    >
                      {inspection.vehicle.name}
                    </a>
                  </div>
                }
              />

              <DetailFieldRow
                label="Form Title"
                value={formatEmptyValueWithUnknown(
                  inspection.snapshotFormTitle,
                )}
              />
              <DetailFieldRow
                label="Form Description"
                value={formatEmptyValueWithUnknown(
                  inspection.snapshotFormDescription,
                )}
              />
              <DetailFieldRow
                label="Started"
                value={formatDateTime(inspection.inspectionStartTime)}
              />
              <DetailFieldRow
                label="Submitted"
                value={formatDateTime(inspection.inspectionEndTime)}
              />
              <DetailFieldRow
                label="Inspection Duration"
                value={calculateDuration(
                  inspection.inspectionStartTime,
                  inspection.inspectionEndTime,
                )}
              />
              <DetailFieldRow
                label="Submitted By"
                value={
                  <div className="flex items-center gap-2">
                    <div className="w-6 h-6 bg-blue-600 rounded-full flex items-center justify-center">
                      <span className="text-xs font-medium text-white">
                        {inspection.technician.firstName &&
                        inspection.technician.lastName
                          ? `${inspection.technician.firstName} ${inspection.technician.lastName}`
                              .split(" ")
                              .map((n: string) => n[0])
                              .join("")
                              .toUpperCase()
                          : "U"}
                      </span>
                    </div>
                    <a
                      href={`/users/${inspection.technicianID}`}
                      className="text-blue-600 hover:text-blue-800 underline text-sm"
                    >
                      {formatEmptyValueWithUnknown(
                        `${inspection.technician.firstName} ${inspection.technician.lastName}`,
                      )}
                    </a>
                  </div>
                }
              />
              <DetailFieldRow
                label="Odometer Reading"
                value={formatEmptyValueWithUnknown(
                  inspection.odometerReading
                    ? `${inspection.odometerReading.toLocaleString()} km`
                    : null,
                )}
              />
              <DetailFieldRow
                label="Vehicle Condition"
                value={
                  inspection.vehicleConditionLabel ? (
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-medium ${
                        inspection.vehicleConditionEnum === 1
                          ? "bg-green-100 text-green-800"
                          : inspection.vehicleConditionEnum === 2
                            ? "bg-yellow-100 text-yellow-800"
                            : "bg-red-100 text-red-800"
                      }`}
                    >
                      {inspection.vehicleConditionLabel}
                    </span>
                  ) : (
                    formatEmptyValueWithUnknown(
                      inspection.vehicleConditionLabel,
                    )
                  )
                }
              />
              <DetailFieldRow
                label="Notes"
                value={formatEmptyValueWithUnknown(inspection.notes)}
                noBorder
              />
            </FormContainer>
          </div>

          {/* Right Column - Inspection Items */}
          <div className="flex-1">
            <FormContainer
              title="Inspection Items"
              className="max-w-2xl mx-auto"
            >
              <div className="space-y-4">
                <DetailFieldRow
                  label="Total Items"
                  value={inspection.inspectionItems.length}
                />
                <DetailFieldRow
                  label="Passed Items"
                  value={
                    <span className="text-green-600 font-medium">
                      {
                        inspection.inspectionItems.filter(item => item.passed)
                          .length
                      }
                    </span>
                  }
                />
                <DetailFieldRow
                  label="Failed Items"
                  value={
                    <span className="text-red-600 font-medium">
                      {
                        inspection.inspectionItems.filter(item => !item.passed)
                          .length
                      }
                    </span>
                  }
                />
                <DetailFieldRow
                  label="Pass Rate"
                  value={
                    <span className="font-medium">
                      {inspection.inspectionItems.length > 0
                        ? `${Math.round(
                            (inspection.inspectionItems.filter(
                              item => item.passed,
                            ).length /
                              inspection.inspectionItems.length) *
                              100,
                          )}%`
                        : "0%"}
                      `
                    </span>
                  }
                  noBorder
                />
              </div>

              <div className="mt-6 pt-4 border-t border-gray-100">
                <h4 className="text-sm font-medium text-gray-900 mb-3">
                  Individual Items
                </h4>
                <div className="space-y-3">
                  {inspection.inspectionItems.map((item, index) => (
                    <div
                      key={index}
                      className={`p-3 rounded-lg border ${
                        item.passed
                          ? "bg-green-50 border-green-200"
                          : "bg-red-50 border-red-200"
                      }`}
                    >
                      <div className="flex items-start justify-between">
                        <div className="flex-1">
                          <div className="flex items-center gap-2 mb-1">
                            <span
                              className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${
                                item.passed
                                  ? "bg-green-100 text-green-800"
                                  : "bg-red-100 text-red-800"
                              }`}
                            >
                              {item.passed ? "PASS" : "FAIL"}
                            </span>
                            <span className="text-xs text-gray-500">
                              {item.snapshotInspectionFormItemTypeLabel}
                            </span>
                            {item.snapshotIsRequired && (
                              <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-orange-100 text-orange-800">
                                Required
                              </span>
                            )}
                          </div>
                          <h5 className="text-sm font-medium text-gray-900 mb-1">
                            {item.snapshotItemLabel}
                          </h5>
                          {item.snapshotItemDescription && (
                            <p className="text-xs text-gray-600 mb-2">
                              {item.snapshotItemDescription}
                            </p>
                          )}
                          {item.comment && (
                            <div className="text-xs text-gray-700 bg-white p-2 rounded border">
                              <span className="font-medium">Comment:</span>{" "}
                              {item.comment}
                            </div>
                          )}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </FormContainer>
          </div>
        </div>

        {/* Footer */}
        <div className="mt-8 text-center">
          <p className="text-xs text-gray-500">
            Created by{" "}
            {formatEmptyValueWithUnknown(
              `${inspection.technician.firstName} ${inspection.technician.lastName}`,
            )}{" "}
            {format(new Date(inspection.createdAt), "MMM dd, yyyy 'at' h:mma")}
          </p>
        </div>
      </div>
    </div>
  );
}

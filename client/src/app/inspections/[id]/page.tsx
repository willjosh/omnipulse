"use client";
import React from "react";
import { useParams, useRouter } from "next/navigation";
import { ArrowLeft, MoreHorizontal, Printer } from "lucide-react";
import InspectionHeader from "@/features/inspection/components/InspectionHeader";
import { useInspection } from "@/features/inspection/hooks/useInspections";
import PrimaryButton from "@/components/ui/Button/PrimaryButton";
import SecondaryButton from "@/components/ui/Button/SecondaryButton";
import FormContainer from "@/components/ui/Form/FormContainer";
import DetailFieldRow from "@/components/ui/Detail/DetailFieldRow";
import { format } from "date-fns";

export default function InspectionDetailsPage() {
  const params = useParams();
  const router = useRouter();
  const inspectionId = Number(params.id);

  const { inspection, isPending, isError } = useInspection(inspectionId);

  const breadcrumbs = [
    { label: "Inspection History", href: "/inspections" },
    { label: `Submission #${inspectionId}`, href: "#" },
  ];

  const handleBack = () => {
    router.push("/inspections");
  };

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
      <SecondaryButton className="flex items-center gap-2">
        <MoreHorizontal className="w-4 h-4" />
        More options
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
                      {inspection.vehicleName}
                    </a>
                  </div>
                }
              />

              <DetailFieldRow
                label="Form Title"
                value={inspection.snapshotFormTitle}
              />
              {inspection.snapshotFormDescription && (
                <DetailFieldRow
                  label="Form Description"
                  value={inspection.snapshotFormDescription}
                />
              )}
              <DetailFieldRow
                label="Started"
                value={formatDateTime(inspection.inspectionStartTime)}
              />
              <DetailFieldRow
                label="Submitted"
                value={formatDateTime(inspection.inspectionEndTime)}
              />
              <DetailFieldRow
                label="Duration"
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
                        {inspection.technicianName
                          ? inspection.technicianName
                              .split(" ")
                              .map(n => n[0])
                              .join("")
                              .toUpperCase()
                          : "U"}
                      </span>
                    </div>
                    <a
                      href={`/users/${inspection.technicianID}`}
                      className="text-blue-600 hover:text-blue-800 underline text-sm"
                    >
                      {inspection.technicianName || "Unknown User"}
                    </a>
                  </div>
                }
              />
              {inspection.odometerReading && (
                <DetailFieldRow
                  label="Odometer Reading"
                  value={`${inspection.odometerReading.toLocaleString()} km`}
                />
              )}
              <DetailFieldRow
                label="Vehicle Condition"
                value={inspection.vehicleConditionLabel}
              />
              {inspection.notes && (
                <DetailFieldRow
                  label="Notes"
                  value={inspection.notes}
                  noBorder
                />
              )}
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
                  value={inspection.inspectionItemsCount}
                />
                <DetailFieldRow
                  label="Passed Items"
                  value={
                    <span className="text-green-600 font-medium">
                      {inspection.passedItemsCount}
                    </span>
                  }
                />
                <DetailFieldRow
                  label="Failed Items"
                  value={
                    <span className="text-red-600 font-medium">
                      {inspection.failedItemsCount}
                    </span>
                  }
                />
                <DetailFieldRow
                  label="Pass Rate"
                  value={
                    <span className="font-medium">
                      {inspection.inspectionItemsCount > 0
                        ? `${Math.round(
                            (inspection.passedItemsCount /
                              inspection.inspectionItemsCount) *
                              100,
                          )}%`
                        : "0%"}
                    </span>
                  }
                  noBorder
                />
              </div>

              {/* <div className="mt-6 pt-4 border-t border-gray-100">
                <div className="text-center py-4">
                  <div className="text-gray-500 text-sm">
                    Individual inspection items will be displayed here
                  </div>
                  <div className="text-gray-400 text-xs mt-2">
                    When API endpoints are available
                  </div>
                </div>
              </div> */}
            </FormContainer>
          </div>
        </div>

        {/* Footer */}
        <div className="mt-8 text-center">
          <p className="text-xs text-gray-500">
            Created by {inspection.technicianName || "Unknown User"}{" "}
            {format(new Date(inspection.createdAt), "MMM dd, yyyy 'at' h:mma")}
          </p>
        </div>
      </div>
    </div>
  );
}

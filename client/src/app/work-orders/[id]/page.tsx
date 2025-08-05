"use client";

import React from "react";
import { useParams, useRouter } from "next/navigation";
import { Edit, Trash2, Printer } from "lucide-react";
import WorkOrderHeader from "@/features/work-order/components/WorkOrderHeader";
import { useWorkOrder } from "@/features/work-order/hooks/useWorkOrders";
import { useIssue } from "@/features/issue/hooks/useIssues";
import PrimaryButton from "@/components/ui/Button/PrimaryButton";
import SecondaryButton from "@/components/ui/Button/SecondaryButton";

export default function WorkOrderPage() {
  const router = useRouter();
  const params = useParams();
  const workOrderId = Number(params.id);

  const { workOrder, isPending, isError } = useWorkOrder(workOrderId);

  if (isPending) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
            <p className="mt-2 text-gray-600">Loading work order...</p>
          </div>
        </div>
      </div>
    );
  }

  if (isError || !workOrder) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <p className="text-red-600 mb-2">Error loading work order</p>
            <SecondaryButton
              onClick={() => router.push("/work-orders")}
              className="mt-4"
            >
              Back to Work Orders
            </SecondaryButton>
          </div>
        </div>
      </div>
    );
  }

  const handleStatusChange = (status: string) => {
    // TODO: Implement status change logic
    console.log("Status changed to:", status);
  };

  const handleEdit = () => {
    router.push(`/work-orders/${workOrderId}/edit`);
  };

  const handleDelete = () => {
    // TODO: Implement delete logic
    console.log("Delete work order");
  };

  const breadcrumbs = [{ label: "Work Orders", href: "/work-orders" }];

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case "created":
      case "assigned":
        return "bg-green-500";
      case "in_progress":
      case "waiting_parts":
        return "bg-orange-500";
      case "completed":
        return "bg-gray-500";
      case "cancelled":
        return "bg-red-500";
      case "on_hold":
        return "bg-orange-500";
      default:
        return "bg-gray-500";
    }
  };

  const getInitials = (name: string) => {
    return name
      .split(" ")
      .map(word => word.charAt(0))
      .join("")
      .toUpperCase()
      .slice(0, 2);
  };

  const actions = (
    <>
      <SecondaryButton className="flex items-center gap-2">
        <Printer className="w-4 h-4" />
        Print
      </SecondaryButton>
      <PrimaryButton onClick={handleEdit} className="flex items-center gap-2">
        <Edit className="w-4 h-4" />
        Edit
      </PrimaryButton>
      <PrimaryButton
        onClick={handleDelete}
        className="flex items-center gap-2 bg-red-600 hover:bg-red-700 text-white border-red-600 hover:border-red-700"
      >
        <Trash2 className="w-4 h-4" />
        Delete
      </PrimaryButton>
    </>
  );

  return (
    <div className="min-h-screen bg-gray-50">
      <WorkOrderHeader
        title={`Work Order #${workOrder.id}`}
        breadcrumbs={breadcrumbs}
        actions={actions}
      />

      <div className="px-6 py-6">
        <div className="max-w-6xl mx-auto">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 items-start">
            {/* Left Column - Details */}
            <div className="space-y-6">
              {/* Details Section */}
              <div className="bg-white rounded-lg border border-gray-200 p-6">
                <div className="flex items-center justify-between mb-4">
                  <h2 className="text-lg font-semibold text-gray-900">
                    Details
                  </h2>
                </div>

                <div className="space-y-4">
                  <div className="flex justify-between items-center py-2 border-b border-gray-100">
                    <span className="text-sm font-medium text-gray-600">
                      Vehicle
                    </span>
                    <span className="text-sm text-gray-900">
                      {workOrder.vehicleName}
                    </span>
                  </div>

                  <div className="flex justify-between items-center py-2 border-b border-gray-100">
                    <span className="text-sm font-medium text-gray-600">
                      Status
                    </span>
                    <div className="flex items-center gap-2">
                      <div
                        className={`w-2 h-2 rounded-full ${getStatusColor(workOrder.statusLabel)}`}
                      />
                      <span className="text-sm text-gray-900">
                        {workOrder.statusLabel}
                      </span>
                    </div>
                  </div>

                  <div className="flex justify-between items-center py-2 border-b border-gray-100">
                    <span className="text-sm font-medium text-gray-600">
                      Work Order Class
                    </span>
                    <span className="text-sm text-gray-900">
                      {workOrder.workOrderTypeLabel}
                    </span>
                  </div>

                  <div className="flex justify-between items-center py-2 border-b border-gray-100">
                    <span className="text-sm font-medium text-gray-600">
                      Priority
                    </span>
                    <span className="text-sm text-gray-900">
                      {workOrder.priorityLevelLabel}
                    </span>
                  </div>

                  <div className="flex justify-between items-center py-2 border-b border-gray-100">
                    <span className="text-sm font-medium text-gray-600">
                      Assigned To
                    </span>
                    <div className="flex items-center gap-2">
                      {workOrder.assignedToUserName ? (
                        <>
                          <div className="w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center">
                            <span className="text-xs font-medium text-white">
                              {getInitials(workOrder.assignedToUserName)}
                            </span>
                          </div>
                          <span className="text-sm text-blue-600 underline decoration-dotted underline-offset-2">
                            {workOrder.assignedToUserName}
                          </span>
                        </>
                      ) : (
                        <span className="text-sm text-gray-900">—</span>
                      )}
                    </div>
                  </div>

                  <div className="flex justify-between items-center py-2 border-b border-gray-100">
                    <span className="text-sm font-medium text-gray-600">
                      Scheduled Start Date
                    </span>
                    <span className="text-sm text-gray-900">
                      {workOrder.scheduledStartDate || "—"}
                    </span>
                  </div>

                  <div className="flex justify-between items-center py-2 border-b border-gray-100">
                    <span className="text-sm font-medium text-gray-600">
                      Actual Start Date
                    </span>
                    <span className="text-sm text-gray-900">
                      {workOrder.actualStartDate || "—"}
                    </span>
                  </div>

                  <div className="flex justify-between items-center py-2 border-b border-gray-100">
                    <span className="text-sm font-medium text-gray-600">
                      Scheduled Completion Date
                    </span>
                    <span className="text-sm text-gray-900">
                      {workOrder.scheduledCompletionDate || "—"}
                    </span>
                  </div>

                  <div className="flex justify-between items-center py-2 border-b border-gray-100">
                    <span className="text-sm font-medium text-gray-600">
                      Actual Completion Date
                    </span>
                    <span className="text-sm text-gray-900">
                      {workOrder.actualCompletionDate || "—"}
                    </span>
                  </div>

                  <div className="flex justify-between items-center py-2 border-b border-gray-100">
                    <span className="text-sm font-medium text-gray-600">
                      Start Odometer
                    </span>
                    <span className="text-sm text-gray-900">
                      {workOrder.startOdometer.toLocaleString()}
                    </span>
                  </div>

                  <div className="flex justify-between items-center py-2 border-b border-gray-100">
                    <span className="text-sm font-medium text-gray-600">
                      End Odometer
                    </span>
                    <span className="text-sm text-gray-900">
                      {workOrder.endOdometer
                        ? workOrder.endOdometer.toLocaleString()
                        : "—"}
                    </span>
                  </div>

                  <div className="flex justify-between items-center py-2 border-b border-gray-100">
                    <span className="text-sm font-medium text-gray-600">
                      Description
                    </span>
                    <span className="text-sm text-gray-900">
                      {workOrder.description || "—"}
                    </span>
                  </div>
                </div>
              </div>

              {/* Issues Section */}
              <div className="bg-white rounded-lg border border-gray-200 p-6">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">
                  Issues
                </h2>
                {workOrder.issueIDs && workOrder.issueIDs.length > 0 ? (
                  <div className="space-y-2">
                    {workOrder.issueIDs.map(issueId => {
                      const { issue } = useIssue(issueId);
                      if (!issue) return null;

                      return (
                        <div
                          key={issueId}
                          className="flex items-center justify-between p-3 hover:bg-gray-50 rounded-lg transition-colors cursor-pointer"
                          onClick={() => router.push(`/issues/${issueId}`)}
                        >
                          <div className="flex items-center gap-3">
                            <span className="text-sm font-medium text-green-600 underline decoration-dotted underline-offset-2">
                              #{issue.issueNumber} · {issue.title}
                            </span>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                ) : (
                  <div className="text-center py-8">
                    <div className="w-16 h-16 mx-auto mb-4 text-gray-400">
                      <svg
                        fill="none"
                        stroke="currentColor"
                        viewBox="0 0 24 24"
                      >
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          strokeWidth={2}
                          d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
                        />
                      </svg>
                    </div>
                    <p className="text-sm text-gray-500">
                      No issues to show. If this work order resolves any issues,
                      you can add them by editing the work order.
                    </p>
                  </div>
                )}
              </div>
            </div>

            {/* Right Column - Line Items */}
            <div className="bg-white rounded-lg border border-gray-200 p-6 h-fit">
              <h2 className="text-lg font-semibold text-gray-900 mb-4">
                Line Items
              </h2>

              {/* Summary Box */}
              <div className="grid grid-cols-3 gap-4 mb-6 p-4 bg-gray-50 rounded-lg">
                <div className="text-center">
                  <div className="text-sm font-medium text-gray-600">Labor</div>
                  <div className="text-lg font-semibold text-gray-900">
                    ${workOrder.totalLaborCost?.toFixed(2) || "0.00"}
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-sm font-medium text-gray-600">Parts</div>
                  <div className="text-lg font-semibold text-gray-900">
                    ${workOrder.totalItemCost?.toFixed(2) || "0.00"}
                  </div>
                </div>
                <div className="text-center">
                  <div className="text-sm font-medium text-gray-600">Total</div>
                  <div className="text-lg font-semibold text-gray-900">
                    ${workOrder.totalCost?.toFixed(2) || "0.00"}
                  </div>
                </div>
              </div>

              {/* Line Items Table */}
              <div className="mb-6">
                <div className="grid grid-cols-4 gap-4 text-sm font-medium text-gray-600 border-b border-gray-200 pb-2 mb-4">
                  <div>Item</div>
                  <div>Labor</div>
                  <div>Parts</div>
                  <div>Subtotal</div>
                </div>

                {workOrder.workOrderLineItems.length > 0 ? (
                  workOrder.workOrderLineItems.map((item, index) => (
                    <div
                      key={item.id}
                      className="grid grid-cols-4 gap-4 py-3 border-b border-gray-100"
                    >
                      <div className="flex items-center gap-2">
                        <span className="text-blue-600 underline decoration-dotted underline-offset-2">
                          {item.serviceTaskName}
                        </span>
                      </div>
                      <div>${item.laborCost.toFixed(2)}</div>
                      <div>${item.itemCost.toFixed(2)}</div>
                      <div>${item.subTotal.toFixed(2)}</div>
                    </div>
                  ))
                ) : (
                  <div className="text-center py-8 text-gray-500">
                    <p className="text-sm">No line items added yet.</p>
                  </div>
                )}
              </div>

              {/* Cost Summary */}
              <div className="border-t border-gray-200 pt-4">
                <div className="space-y-2 text-right">
                  <div className="flex justify-between">
                    <span className="text-sm text-gray-600">Subtotal</span>
                    <span className="text-sm text-gray-900">
                      + ${workOrder.totalCost?.toFixed(2) || "0.00"}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-sm text-gray-600">Labor</span>
                    <span className="text-sm text-gray-900">
                      ${workOrder.totalLaborCost?.toFixed(2) || "0.00"}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-sm text-gray-600">Parts</span>
                    <span className="text-sm text-gray-900">
                      ${workOrder.totalItemCost?.toFixed(2) || "0.00"}
                    </span>
                  </div>
                  <div className="flex justify-between pt-2 border-t border-gray-200">
                    <span className="text-lg font-semibold text-gray-900">
                      Total
                    </span>
                    <span className="text-lg font-semibold text-gray-900">
                      ${workOrder.totalCost?.toFixed(2) || "0.00"}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

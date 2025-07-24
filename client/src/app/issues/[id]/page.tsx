"use client";
import React from "react";
import { useParams, useRouter } from "next/navigation";
import { Edit, ArrowLeft, Trash2 } from "lucide-react";
import StatusBadge from "@/app/_features/shared/feedback/StatusBadge";
import LoadingSpinner from "@/app/_features/shared/feedback/LoadingSpinner";
import EmptyState from "@/app/_features/shared/feedback/EmptyState";
import DetailFieldRow from "@/app/_features/shared/detail/DetailFieldRow";
import { useIssue } from "@/app/_hooks/issue/useIssues";
import { getTimeToResolve } from "@/app/_utils/issueEnumHelper";
import { useDeleteIssue } from "@/app/_hooks/issue/useIssues";

const IssueDetailsPage = () => {
  const params = useParams();
  const router = useRouter();
  const id = params?.id ? Number(params.id) : undefined;
  const isValidId = typeof id === "number" && !isNaN(id);
  const { mutate: deleteIssue, isPending: isDeleting } = useDeleteIssue();
  const { data: issue, isPending, isError } = useIssue(isValidId ? id : 0);

  if (!isValidId) {
    return (
      <EmptyState
        title="Invalid Issue ID"
        message="The issue ID in the URL is invalid."
      />
    );
  }

  if (isPending) {
    return (
      <div className="flex justify-center items-center h-96">
        <LoadingSpinner />
      </div>
    );
  }

  if (isError || !issue) {
    return (
      <EmptyState
        title="Issue not found"
        message="The issue you are looking for does not exist or could not be loaded."
      />
    );
  }

  const handleBack = () => {
    router.push("/issues");
  };

  const handleEdit = () => {
    router.push(`/issues/${issue.id}/edit`);
  };

  const handleDelete = () => {
    if (
      window.confirm(
        "Are you sure you want to delete this issue? This action cannot be undone.",
      )
    ) {
      deleteIssue(issue.id, {
        onSuccess: () => {
          router.push("/issues");
        },
      });
    }
  };

  return (
    <div className="min-h-screen max-w-7xl shadow border-b border-gray-200 bg-gray-50 mx-auto">
      <div className="bg-white">
        <div className="px-6 py-4">
          <div className="flex items-center space-x-4 mb-4">
            <button
              onClick={handleBack}
              className="flex items-center text-gray-600 hover:text-gray-900"
            >
              <ArrowLeft className="w-4 h-4 mr-1" />
              <span className="text-sm">Issues</span>
            </button>
          </div>
          <div className="flex items-start justify-between">
            <div>
              <h1 className="text-2xl font-bold text-gray-900 mb-1">
                {issue.title}
              </h1>
              <p className="text-gray-600 mb-2">
                Issue #{issue.issueNumber} • {issue.vehicleName} •{" "}
                {issue.categoryLabel} • {issue.priorityLevelLabel}
              </p>
              <div className="flex items-center space-x-4 text-sm">
                <StatusBadge status={issue.statusLabel} />
                <span className="text-gray-600">
                  Reported by {issue.reportedByUserName}
                </span>
                <span className="text-gray-600">
                  {issue.reportedDate ? issue.reportedDate : "Unknown"}
                </span>
              </div>
            </div>
            <div className="flex items-center space-x-3">
              <button
                onClick={handleEdit}
                className="inline-flex items-center px-4 py-2 rounded-lg bg-blue-600 text-white font-semibold hover:bg-blue-700 transition"
              >
                <Edit className="w-4 h-4 mr-2" />
                Edit
              </button>
              <button
                onClick={handleDelete}
                className="inline-flex items-center px-4 py-2 rounded-lg bg-red-600 text-white font-semibold hover:bg-red-700 transition"
                disabled={isDeleting}
              >
                <Trash2 className="w-4 h-4 mr-2" />
                {isDeleting ? "Deleting..." : "Delete"}
              </button>
            </div>
          </div>
        </div>
        <div className="w-full px-6 pb-6">
          <div className="flex flex-col md:flex-row md:gap-6">
            {/* Left column: Details card (50% width) */}
            <div className="w-full md:w-1/2 flex flex-col gap-6">
              <div className="bg-white rounded-3xl border border-gray-200">
                <div className="p-4 border-b border-gray-200">
                  <h2 className="text-lg font-semibold text-gray-900">
                    Details
                  </h2>
                </div>
                <div className="p-3 space-y-2">
                  <DetailFieldRow label="Issue #" value={issue.issueNumber} />
                  <DetailFieldRow
                    label="Status"
                    value={<StatusBadge status={issue.statusLabel} />}
                  />
                  <DetailFieldRow
                    label="Priority"
                    value={issue.priorityLevelLabel}
                  />
                  <DetailFieldRow
                    label="Category"
                    value={issue.categoryLabel}
                  />
                  <DetailFieldRow
                    label="Vehicle"
                    value={
                      <span className="text-blue-700 font-medium">
                        {issue.vehicleName}
                      </span>
                    }
                  />
                  <DetailFieldRow
                    label="Assigned To"
                    value={issue.resolvedByUserName || "Unassigned"}
                  />
                  <DetailFieldRow
                    label="Reported By"
                    value={issue.reportedByUserName}
                  />
                  <DetailFieldRow
                    label="Reported Date"
                    value={issue.reportedDate ? issue.reportedDate : "Unknown"}
                  />
                  <DetailFieldRow
                    label="Description"
                    value={
                      issue.description || (
                        <span className="text-gray-400">
                          No description provided.
                        </span>
                      )
                    }
                  />
                </div>
              </div>
            </div>
            {/* Right column: Resolution Details and Timeline (50% width) */}
            <div className="w-full md:w-1/2 flex flex-col gap-6 mt-6 md:mt-0">
              {/* Resolution Details Card */}
              <div className="bg-white rounded-3xl border border-gray-200">
                <div className="p-4 border-b border-gray-200">
                  <h2 className="text-lg font-semibold text-gray-900">
                    Resolution Details
                  </h2>
                </div>
                <div className="p-3 space-y-2">
                  <DetailFieldRow
                    label="Resolved Date"
                    value={issue.resolvedDate ? issue.resolvedDate : "Unknown"}
                  />
                  <DetailFieldRow
                    label="Time to Resolve"
                    value={getTimeToResolve(
                      issue.reportedDate,
                      issue.resolvedDate,
                    )}
                  />
                  <DetailFieldRow
                    label="Resolution Notes"
                    value={
                      issue.resolutionNotes || (
                        <span className="text-gray-400">
                          No resolution notes provided.
                        </span>
                      )
                    }
                  />
                </div>
              </div>
              {/* Timeline Card */}
              <div className="bg-white rounded-3xl border border-gray-200">
                <div className="p-4 border-b border-gray-200">
                  <h2 className="text-lg font-semibold text-gray-900">
                    Timeline
                  </h2>
                </div>
                <div className="p-3 text-gray-900 text-sm">
                  <ol className="relative border-l border-gray-200 ml-3">
                    {/* Issue reported event */}
                    <li className="mb-8 ml-6">
                      <span className="absolute -left-3 flex items-center justify-center w-6 h-6 bg-blue-100 rounded-full ring-8 ring-white">
                        <svg
                          className="w-3 h-3 text-blue-600"
                          fill="currentColor"
                          viewBox="0 0 20 20"
                        >
                          <path d="M10 2a8 8 0 100 16 8 8 0 000-16zm1 11H9v-2h2v2zm0-4H9V7h2v2z" />
                        </svg>
                      </span>
                      <h3 className="font-semibold leading-tight">
                        Issue reported
                      </h3>
                      <p className="text-xs text-gray-500">
                        {issue.reportedDate ? issue.reportedDate : "Unknown"} by{" "}
                        {issue.reportedByUserName}
                      </p>
                    </li>
                    {/* Issue resolved event, only if resolved */}
                    {issue.resolvedDate && (
                      <li className="ml-6">
                        <span className="absolute -left-3 flex items-center justify-center w-6 h-6 bg-green-100 rounded-full ring-8 ring-white">
                          <svg
                            className="w-3 h-3 text-green-600"
                            fill="currentColor"
                            viewBox="0 0 20 20"
                          >
                            <path d="M16.707 5.293a1 1 0 00-1.414 0L9 11.586 6.707 9.293a1 1 0 00-1.414 1.414l3 3a1 1 0 001.414 0l7-7a1 1 0 000-1.414z" />
                          </svg>
                        </span>
                        <h3 className="font-semibold leading-tight">
                          Issue resolved
                        </h3>
                        <p className="text-xs text-gray-500">
                          {issue.resolvedDate} by{" "}
                          {issue.resolvedByUserName || "Unassigned"}
                        </p>
                      </li>
                    )}
                  </ol>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default IssueDetailsPage;

import React, { useMemo } from "react";
import FormContainer from "@/components/ui/Form/FormContainer";
import { Checkbox } from "@mui/material";
import { useIssues } from "@/features/issue/hooks/useIssues";
import { IssueWithLabels } from "@/features/issue/types/issueType";

export interface WorkOrderIssuesFormValues {
  issueIdList: number[];
}

interface WorkOrderIssuesFormProps {
  value: WorkOrderIssuesFormValues;
  errors: Partial<Record<keyof WorkOrderIssuesFormValues, string>>;
  onChange: (field: keyof WorkOrderIssuesFormValues, value: number[]) => void;
  disabled?: boolean;
  vehicleID: number;
}

const WorkOrderIssuesForm: React.FC<WorkOrderIssuesFormProps> = ({
  value,
  errors,
  onChange,
  disabled = false,
  vehicleID,
}) => {
  // Fetch issues for the selected vehicle
  const { issues, isPending: isLoadingIssues } = useIssues({
    PageNumber: 1,
    PageSize: 100, // Get all issues for the vehicle
    Search: "",
  });

  // Filter issues by vehicle - only show open issues
  const vehicleIssues = useMemo(() => {
    if (!vehicleID) return [];

    return issues.filter((issue: IssueWithLabels) => {
      // First filter by vehicle (convert to numbers for comparison)
      const issueVehicleID = Number(issue.vehicleID);
      const selectedVehicleID = Number(vehicleID);

      if (issueVehicleID !== selectedVehicleID) {
        return false;
      }

      // Only include open issues (status = 1)
      return issue.statusEnum === 1; // OPEN
    });
  }, [issues, vehicleID]);

  const handleIssueToggle = (issueId: number) => {
    const currentList = value.issueIdList || [];
    const newList = currentList.includes(issueId)
      ? currentList.filter(id => id !== issueId)
      : [...currentList, issueId];
    onChange("issueIdList", newList);
  };

  if (!vehicleID) {
    return (
      <FormContainer title="Issues" className="mt-6 max-w-4xl mx-auto w-full">
        <div className="text-gray-500 text-center py-4">
          Please select a vehicle first to view related issues.
        </div>
      </FormContainer>
    );
  }

  return (
    <FormContainer title="Issues" className="mt-6 max-w-4xl mx-auto w-full">
      {isLoadingIssues ? (
        <div className="text-gray-500 text-center py-4">Loading issues...</div>
      ) : vehicleIssues.length === 0 ? (
        <div className="text-gray-500 text-center py-4">
          No open issues found for this vehicle.
        </div>
      ) : (
        <div className="space-y-4">
          {/* Section header */}
          <div className="border-b border-gray-200 pb-2">
            <p className="text-xs text-gray-500">
              Select issues that will be addressed by this work order
            </p>
          </div>

          {/* Issues Table */}
          <div
            className="border border-gray-200 rounded-lg overflow-hidden"
            style={{
              height:
                vehicleIssues.length === 0
                  ? "200px" // Empty state - smaller height
                  : vehicleIssues.length <= 8
                    ? `${Math.max(200, vehicleIssues.length * 60 + 80)}px` // Dynamic height based on number of issues
                    : "500px", // Many issues - reasonable max height with scroll
            }}
          >
            <table className="w-full">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    <Checkbox
                      checked={
                        vehicleIssues.length > 0 &&
                        vehicleIssues.every(issue =>
                          value.issueIdList?.includes(issue.id),
                        )
                      }
                      indeterminate={
                        vehicleIssues.some(issue =>
                          value.issueIdList?.includes(issue.id),
                        ) &&
                        !vehicleIssues.every(issue =>
                          value.issueIdList?.includes(issue.id),
                        )
                      }
                      onChange={() => {
                        const allSelected = vehicleIssues.every(issue =>
                          value.issueIdList?.includes(issue.id),
                        );
                        if (allSelected) {
                          // If all are selected, deselect all
                          onChange("issueIdList", []);
                        } else {
                          // If some or none are selected, select all
                          const allIssueIds = vehicleIssues.map(
                            issue => issue.id,
                          );
                          onChange("issueIdList", allIssueIds);
                        }
                      }}
                      disabled={disabled}
                      size="small"
                    />
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Priority
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Issue
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Summary
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Category
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Reported
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {vehicleIssues.map((issue: IssueWithLabels) => (
                  <tr
                    key={issue.id}
                    className="hover:bg-blue-50 cursor-pointer transition-colors duration-150"
                    onClick={() => handleIssueToggle(issue.id)}
                  >
                    <td className="px-4 py-3 whitespace-nowrap">
                      <Checkbox
                        checked={value.issueIdList?.includes(issue.id) || false}
                        onChange={() => handleIssueToggle(issue.id)}
                        disabled={disabled}
                        size="small"
                        onClick={(e: React.MouseEvent) => e.stopPropagation()}
                      />
                    </td>
                    <td className="px-4 py-3 whitespace-nowrap">
                      <span
                        className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${
                          issue.priorityLevelEnum === 3 || // HIGH
                          issue.priorityLevelEnum === 4 // CRITICAL
                            ? "bg-red-100 text-red-800"
                            : issue.priorityLevelEnum === 2 // MEDIUM
                              ? "bg-yellow-100 text-yellow-800"
                              : "bg-green-100 text-green-800"
                        }`}
                      >
                        {issue.priorityLevelLabel}
                      </span>
                    </td>
                    <td className="px-4 py-3 whitespace-nowrap text-sm font-medium text-gray-900">
                      #{issue.issueNumber}
                    </td>
                    <td className="px-4 py-3 text-sm text-gray-900 max-w-xs truncate">
                      {issue.title}
                    </td>
                    <td className="px-4 py-3 whitespace-nowrap">
                      <span
                        className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${
                          issue.statusEnum === 1 // OPEN
                            ? "bg-blue-100 text-blue-800"
                            : issue.statusEnum === 2 // IN_PROGRESS
                              ? "bg-orange-100 text-orange-800"
                              : "bg-gray-100 text-gray-800"
                        }`}
                      >
                        {issue.statusLabel}
                      </span>
                    </td>
                    <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-500">
                      {issue.categoryLabel}
                    </td>
                    <td className="px-4 py-3 whitespace-nowrap text-sm text-gray-500">
                      {issue.reportedDate
                        ? new Date(issue.reportedDate).toLocaleDateString()
                        : "—"}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Summary */}
          <div className="text-sm text-gray-600">
            {value.issueIdList?.length || 0} of {vehicleIssues.length} issues
            selected
          </div>
        </div>
      )}
      {errors.issueIdList && (
        <span className="text-sm text-red-500 mt-1">{errors.issueIdList}</span>
      )}
    </FormContainer>
  );
};

export default WorkOrderIssuesForm;

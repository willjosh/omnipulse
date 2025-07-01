"use client";

import { Box, Paper, Typography, Avatar, Link as MuiLink } from "@mui/material";
import { useWorkOrderFormStore } from "../store/workOrderFormStore";

const FieldRow = ({
  label,
  value,
}: {
  label: string;
  value: React.ReactNode;
}) => (
  <Box
    display="flex"
    justifyContent="space-between"
    alignItems="center"
    px={2}
    py={1.5}
    borderBottom="1px solid #eee"
  >
    <Typography color="text.secondary" fontSize={14} sx={{ flex: 1 }}>
      {label}
    </Typography>
    <Box sx={{ flex: 2, ml: 10 }}>
      {value || <Typography color="text.disabled">—</Typography>}
    </Box>
  </Box>
);

export const WorkOrderDetailsPanel = () => {
  const { formData } = useWorkOrderFormStore();
  const { details, scheduling } = formData;

  return (
    <Paper sx={{ width: "35%", borderRadius: 2, overflow: "hidden" }}>
      <Box px={2} pt={2}>
        <Typography variant="h6" fontWeight="bold">
          Details
        </Typography>
        <Typography variant="body2" fontWeight="bold" mt={0.5}>
          All Fields
        </Typography>
      </Box>

      <FieldRow
        label="Vehicle"
        value={
          details.vehicleId ? (
            <Box display="flex" alignItems="center" gap={1}>
              <MuiLink href="#" underline="hover">
                {details.vehicleId}
              </MuiLink>
            </Box>
          ) : null
        }
      />

      <FieldRow
        label="Status"
        value={
          <Box display="flex" alignItems="center" gap={1}>
            <Box width={10} height={10} borderRadius="50%" bgcolor="cyan" />
            <Typography fontSize={14}>Open</Typography>
            <MuiLink href="#" ml={20} fontSize={14}>
              History
            </MuiLink>
          </Box>
        }
      />

      <FieldRow
        label="Repair Priority Class"
        value={details.repairPriorityClass}
      />

      <FieldRow
        label="Issued By"
        value={
          scheduling.issuedBy && (
            <Box display="flex" alignItems="center" gap={1}>
              <Avatar
                sx={{ width: 24, height: 24, bgcolor: "#ff6600", fontSize: 12 }}
              >
                {scheduling.issuedBy
                  .split(" ")
                  .map(n => n[0])
                  .join("")}
              </Avatar>
              <MuiLink href="#" underline="hover" fontSize={14}>
                {scheduling.issuedBy}
              </MuiLink>
            </Box>
          )
        }
      />

      <FieldRow label="Assigned To" value={details.assignedTo} />
      <FieldRow
        label="Issue Date"
        value={
          scheduling.issueDate
            ? scheduling.issueDate instanceof Date
              ? scheduling.issueDate.toLocaleDateString()
              : scheduling.issueDate
            : ""
        }
      />

      <FieldRow
        label="Scheduled Start Date"
        value={
          scheduling.scheduledStartDate
            ? scheduling.scheduledStartDate instanceof Date
              ? scheduling.scheduledStartDate.toLocaleDateString()
              : scheduling.scheduledStartDate
            : ""
        }
      />
      <FieldRow
        label="Actual Start Date"
        value={
          scheduling.actualStartDate
            ? scheduling.actualStartDate instanceof Date
              ? scheduling.actualStartDate.toLocaleDateString()
              : scheduling.actualStartDate
            : ""
        }
      />
      <FieldRow
        label="Expected Completion Date"
        value={
          scheduling.expectedCompletionDate
            ? scheduling.expectedCompletionDate instanceof Date
              ? scheduling.expectedCompletionDate.toLocaleDateString()
              : scheduling.expectedCompletionDate
            : ""
        }
      />
      <FieldRow
        label="Actual Completion Date"
        value={
          scheduling.actualCompletionDate
            ? scheduling.actualCompletionDate instanceof Date
              ? scheduling.actualCompletionDate.toLocaleDateString()
              : scheduling.actualCompletionDate
            : ""
        }
      />
      <FieldRow label="Description" value={details.description} />
      <FieldRow label="Vendor" value={details.vendor} />
      <FieldRow label="Invoice Number" value={details.invoiceNumber} />
      <FieldRow label="PO Number" value={details.poNumber} />
    </Paper>
  );
};

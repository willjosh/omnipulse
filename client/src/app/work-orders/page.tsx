"use client";

import React, { useState } from "react";
import {
  Box,
  Button,
  Typography,
  IconButton,
  Tabs,
  Tab,
  TextField,
  Table,
  TableHead,
  TableRow,
  TableCell,
  Divider,
  TableBody,
} from "@mui/material";
import { useRouter } from "next/navigation";
import { Plus, Filter, Search, Settings, ChevronDown } from "lucide-react";
import { format } from "date-fns";
import Link from "next/link";
import { useWorkOrderListStore } from "@/app/_features/work-order/store/workOrderListStore";

const WorkOrdersPage: React.FC = () => {
  const [status, setStatus] = useState("Open");

  const handleStatusChange = (_: any, newValue: string) => {
    setStatus(newValue);
  };

  const { workOrders } = useWorkOrderListStore();

  const router = useRouter();

  return (
    <Box p={0}>
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={2}
        p={2}
      >
        <Typography variant="h5" fontWeight="bold">
          Work Orders
        </Typography>
        <Box display="flex" gap={1}>
          <Button
            variant="contained"
            startIcon={<Plus />}
            component={Link}
            href="/work-orders/new"
          >
            ADD WORK ORDER
          </Button>
          <IconButton>
            <Settings />
          </IconButton>
        </Box>
      </Box>
      <Tabs
        value={status}
        onChange={handleStatusChange}
        textColor="primary"
        indicatorColor="primary"
      >
        <Tab label="ALL" value="All" />
        <Tab label="OPEN" value="Open" />
        <Tab label="PENDING" value="Pending" />
        <Tab label="COMPLETED" value="Completed" />
      </Tabs>
      <Box display="flex" alignItems="center" gap={1} mt={2}>
        <TextField
          size="small"
          placeholder="Search"
          variant="outlined"
          sx={{ borderRadius: 5, background: "#fff", width: 250, ml: 2 }}
          InputProps={{ sx: { borderRadius: "50px" } }}
        />
        {["Status", "Vehicle", "Vehicle Group", "Service Tasks"].map(label => (
          <Button
            key={label}
            variant="outlined"
            endIcon={<ChevronDown size={16} />}
            sx={{
              borderRadius: 5,
              textTransform: "none",
              background: "#f5f5f5",
            }}
          >
            {label}
          </Button>
        ))}
        <Button
          variant="outlined"
          startIcon={<Filter />}
          sx={{ borderRadius: 5 }}
        >
          Filters
        </Button>
      </Box>
      <Box overflow="auto" mt={2}>
        <Divider sx={{ mb: 1 }} />
        <Table>
          <TableHead>
            <TableRow sx={{ whiteSpace: "nowrap" }}>
              <TableCell>Vehicle</TableCell>
              <TableCell>Number</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Repair Priority Class</TableCell>
              <TableCell>Service Tasks</TableCell>
              <TableCell>Resolved Issues</TableCell>
              <TableCell>Issue Date</TableCell>
              <TableCell>Expected Completion Date</TableCell>
              <TableCell>Assigned To</TableCell>
              <TableCell>Watchers</TableCell>
              <TableCell>Operator</TableCell>
              <TableCell>Total Cost</TableCell>
              <TableCell>Labels</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {workOrders.map(order => {
              const d = order.data;

              return (
                <TableRow
                  key={order.id}
                  hover
                  sx={{
                    cursor: "pointer",
                    "&:hover": { backgroundColor: "#f0f0f0" },
                  }}
                  onClick={() => router.push(`/work-orders/${order.id}`)}
                >
                  <TableCell>{d.details.vehicleId}</TableCell>
                  <TableCell>#{order.number}</TableCell>
                  <TableCell>{d.details.status}</TableCell>
                  <TableCell>{d.details.repairPriorityClass}</TableCell>
                  <TableCell>—</TableCell>
                  <TableCell>—</TableCell>
                  <TableCell>
                    {d.scheduling.issueDate
                      ? format(new Date(d.scheduling.issueDate), "MM/dd/yyyy")
                      : "—"}
                  </TableCell>
                  <TableCell>
                    {d.scheduling.expectedCompletionDate
                      ? format(
                          new Date(d.scheduling.expectedCompletionDate),
                          "MM/dd/yyyy",
                        )
                      : "—"}
                  </TableCell>
                  <TableCell>{d.details.assignedTo || "—"}</TableCell>
                  <TableCell>1 watcher</TableCell>
                  <TableCell>Jacob Silva</TableCell>
                  <TableCell>RM0.00</TableCell>
                  <TableCell>{d.details.labels || "—"}</TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
        {workOrders.length === 0 && (
          <Box
            display="flex"
            flexDirection="column"
            alignItems="center"
            justifyContent="center"
            mt={10}
          >
            <Search size={64} color="#1976d2" />
            <Typography variant="body1" mt={2}>
              No results to show.
            </Typography>
            <Typography variant="body2" mb={3}>
              Work Orders are used to plan and complete service needed for a
              particular vehicle.
            </Typography>
            <Button
              variant="contained"
              startIcon={<Plus />}
              component={Link}
              href="/work-orders/new"
            >
              ADD WORK ORDER
            </Button>
          </Box>
        )}
      </Box>
    </Box>
  );
};

export default WorkOrdersPage;

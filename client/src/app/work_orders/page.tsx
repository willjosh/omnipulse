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
} from "@mui/material";
import { Plus, Filter, Search, Settings, ChevronDown } from "lucide-react";

const WorkOrdersPage: React.FC = () => {
  const [status, setStatus] = useState("Open");
  const rows: any[] = [];

  const handleStatusChange = (_: any, newValue: string) => {
    setStatus(newValue);
  };

  return (
    <Box p={0}>
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={2}
      >
        <Typography variant="h5">Work Orders</Typography>
        <Box display="flex" gap={1}>
          <Button variant="contained" startIcon={<Plus />}>
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
          sx={{ borderRadius: 5, background: "#fff", width: 250 }}
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
        </Table>
        {rows.length === 0 && (
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
            <Button variant="contained" startIcon={<Plus />}>
              ADD WORK ORDER
            </Button>
          </Box>
        )}
      </Box>
    </Box>
  );
};

export default WorkOrdersPage;

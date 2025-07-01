"use client";

import React from "react";
import {
  Box,
  Button,
  Typography,
  TextField,
  Menu,
  MenuItem,
  IconButton,
  Paper,
  Autocomplete,
  Divider,
  Checkbox,
} from "@mui/material";
import { ChevronLeft, ChevronDown } from "lucide-react";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { DatePicker } from "@mui/x-date-pickers";

const WorkOrderHeader: React.FC = () => {
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);

  const handleMenuClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const vehicleOptions = ["1100 [2018 Toyota Prius]", "2100 [2016 Ford F-150]"];
  const [vehicles, setVehicles] = React.useState<string | null>(null);

  const statusOptions = ["Open", "Pending", "Completed"];
  const [status, setStatus] = React.useState<string | null>(null);

  const repairPriorityOptions = ["Scheduled", "Non-Scheduled", "Emergency"];
  const [repairPriority, setRepairPriority] = React.useState<string | null>(
    null,
  );

  const userOptions = ["Licht Potato", "Andy Miller", "Carlos Garcia"];
  const timeOptions = [];
  for (let hour = 0; hour < 24; hour++) {
    timeOptions.push(`${hour % 12 || 12}:00${hour < 12 ? "am" : "pm"}`);
    timeOptions.push(`${hour % 12 || 12}:30${hour < 12 ? "am" : "pm"}`);
  }

  const [issueDate, setIssueDate] = React.useState<Date | null>(new Date());
  const [issueTime, setIssueTime] = React.useState<string | null>("");
  const [issuedBy, setIssuedBy] = React.useState<string | null>(null);

  const [scheduledStartDate, setScheduledStartDate] =
    React.useState<Date | null>(new Date());
  const [scheduledStartTime, setScheduledStartTime] = React.useState<
    string | null
  >("");
  const [actualStartDate, setActualStartDate] = React.useState<Date | null>(
    new Date(),
  );
  const [actualStartTime, setActualStartTime] = React.useState<string | null>(
    "",
  );
  const [sendReminder, setSendReminder] = React.useState(false);

  const [expectedCompletionDate, setExpectedCompletionDate] =
    React.useState<Date | null>(new Date());
  const [expectedCompletionTime, setExpectedCompletionTime] = React.useState<
    string | null
  >("");
  const [actualCompletionDate, setActualCompletionDate] =
    React.useState<Date | null>(new Date());
  const [actualCompletionTime, setActualCompletionTime] = React.useState<
    string | null
  >("");
  const [useStartOdometer, setUseStartOdometer] = React.useState(false);

  const assignedToOptions = ["Licht Potato", "Andy Miller", "Carlos Garcia"];
  const labelOptions = ["Test"];
  const vendorOptions = ["Elite Tire and Service Inc."];

  const [assignedTo, setAssignedTo] = React.useState<string | null>(null);
  const [labels, setLabels] = React.useState<string | null>(null);
  const [vendor, setVendor] = React.useState<string | null>(null);
  const [invoiceNumber, setInvoiceNumber] = React.useState<
    number | string | null
  >(0);
  const [poNumber, setPoNumber] = React.useState<number | string | null>(0);

  return (
    <Box>
      <Box
        sx={{
          position: "sticky",
          top: 0,
          left: 0,
          right: 0,
          zIndex: 1000,
          bgcolor: "#fff",
          borderBottom: "1px solid #e0e0e0",
        }}
      >
        <Box display="flex" alignItems="center" pt={1} pl={1}>
          <IconButton>
            <ChevronLeft size={16} />
          </IconButton>
          <Typography variant="body2">Work Orders</Typography>
        </Box>
        <Box
          display="flex"
          justifyContent="space-between"
          alignItems="center"
          pb={1.5}
        >
          <Box display="flex" alignItems="center" gap={2}>
            <Typography variant="h5" pl={2.5}>
              New Work Order
            </Typography>
            <TextField
              size="small"
              type="number"
              sx={{ width: 150, height: 40 }}
            />
          </Box>
          <Box display="flex" alignItems="center" gap={1} pr={2}>
            <Button
              color="primary"
              sx={{ textTransform: "none", minHeight: 36 }}
            >
              Cancel
            </Button>
            <Button
              variant="outlined"
              endIcon={<ChevronDown size={16} />}
              onClick={handleMenuClick}
              sx={{ textTransform: "none", minHeight: 36 }}
            >
              Save and ...
            </Button>
            <Menu
              anchorEl={anchorEl}
              open={Boolean(anchorEl)}
              onClose={handleMenuClose}
            >
              <MenuItem onClick={handleMenuClose}>
                Save and Continue Editing
              </MenuItem>
              <MenuItem onClick={handleMenuClose}>
                Save and Add Another
              </MenuItem>
            </Menu>
            <Button
              variant="contained"
              sx={{ textTransform: "none", minHeight: 36 }}
            >
              Save Work Order
            </Button>
          </Box>
        </Box>
      </Box>
      <Box p={2}>
        <Paper elevation={1} sx={{ p: 2, mx: 25, backgroundColor: "#fff" }}>
          <Typography variant="h6" fontWeight="bold" mb={3}>
            Details
          </Typography>

          {/* Vehicle Field */}
          <Typography variant="body1" fontWeight="medium" mb={1}>
            Vehicle <span style={{ color: "red" }}>*</span>
          </Typography>
          <Autocomplete
            options={vehicleOptions}
            value={vehicles}
            onChange={(e, newValue) => setVehicles(newValue)}
            renderInput={params => (
              <TextField {...params} placeholder="Please select" size="small" />
            )}
          />

          {/* Status Field */}
          <Typography variant="body1" fontWeight="medium" mb={1} mt={2}>
            Status <span style={{ color: "red" }}>*</span>
          </Typography>
          <Autocomplete
            options={statusOptions}
            value={status}
            onChange={(e, newValue) => setStatus(newValue)}
            renderInput={params => (
              <TextField {...params} placeholder="Please select" size="small" />
            )}
          />

          {/* Repair Priority Class Field */}
          <Typography variant="body1" fontWeight="medium" mb={1} mt={2}>
            Repair Priority Class <span style={{ color: "red" }}>*</span>
          </Typography>
          <Autocomplete
            options={repairPriorityOptions}
            value={repairPriority}
            onChange={(e, newValue) => setRepairPriority(newValue)}
            renderInput={params => (
              <TextField {...params} placeholder="Please select" size="small" />
            )}
          />
          <Typography variant="body2" mt={0.5} mb={1}>
            Repair Priority Class (VMRS Code Key 16) is a simple way to classify
            whether a service or repair was scheduled, non-scheduled, or an
            emergency.
          </Typography>

          {/* Issue Date and Issued By Row */}
          <Box>
            <Box display="flex" gap={2} mb={0.5} mt={2}>
              <Box flex={3}>
                <Typography variant="body1" fontWeight="medium">
                  Issue Date <span style={{ color: "red" }}>*</span>
                </Typography>
              </Box>
              <Box flex={2}></Box>
              <Box flex={5}>
                <Typography variant="body1" fontWeight="medium">
                  Issued By
                </Typography>
              </Box>
            </Box>

            <Box display="flex" gap={2} alignItems="center">
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  value={issueDate}
                  onChange={newValue => setIssueDate(newValue)}
                  slotProps={{ textField: { size: "small" } }}
                  sx={{ flex: 3 }}
                />
              </LocalizationProvider>

              <Autocomplete
                options={timeOptions}
                value={issueTime}
                onChange={(e, newValue) => setIssueTime(newValue)}
                renderInput={params => (
                  <TextField
                    {...params}
                    placeholder="Select time"
                    size="small"
                  />
                )}
                sx={{ flex: 2 }}
                ListboxProps={{ style: { maxHeight: 200, overflowY: "auto" } }}
              />

              <Autocomplete
                options={userOptions}
                value={issuedBy}
                onChange={(e, newValue) => setIssuedBy(newValue)}
                renderInput={params => (
                  <TextField
                    {...params}
                    placeholder="Select user"
                    size="small"
                  />
                )}
                sx={{ flex: 5 }}
              />
            </Box>
          </Box>

          {/* Scheduled Start Date and Actual Start Date Row */}
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <Divider sx={{ my: 2 }} />
            <Box display="flex" gap={2} mb={0.5}>
              <Box flex={3}>
                <Typography variant="body1" fontWeight="medium">
                  Scheduled Start Date
                </Typography>
              </Box>
              <Box flex={2}></Box>
              <Box flex={5}>
                <Typography variant="body1" fontWeight="medium">
                  Actual Start Date
                </Typography>
              </Box>
            </Box>

            <Box display="flex" gap={2} alignItems="center">
              <DatePicker
                value={scheduledStartDate}
                onChange={newValue => setScheduledStartDate(newValue)}
                slotProps={{ textField: { size: "small" } }}
                sx={{ flex: 3 }}
              />

              <Autocomplete
                options={timeOptions}
                value={scheduledStartTime}
                onChange={(e, newValue) => setScheduledStartTime(newValue)}
                renderInput={params => (
                  <TextField
                    {...params}
                    placeholder="Select time"
                    size="small"
                  />
                )}
                sx={{ flex: 2 }}
                ListboxProps={{ style: { maxHeight: 200, overflowY: "auto" } }}
              />

              <Box sx={{ flex: 5 }} display="flex" gap={2}>
                <DatePicker
                  value={actualStartDate}
                  onChange={newValue => setActualStartDate(newValue)}
                  slotProps={{ textField: { size: "small" } }}
                  sx={{ flex: 3 }}
                />
                <Autocomplete
                  options={timeOptions}
                  value={actualStartTime}
                  onChange={(e, newValue) => setActualStartTime(newValue)}
                  renderInput={params => (
                    <TextField
                      {...params}
                      placeholder="Select time"
                      size="small"
                    />
                  )}
                  sx={{ flex: 2 }}
                  ListboxProps={{
                    style: { maxHeight: 200, overflowY: "auto" },
                  }}
                />
              </Box>
            </Box>
            <Box display="flex" alignItems="center" mt={2}>
              <Checkbox
                checked={sendReminder}
                onChange={e => setSendReminder(e.target.checked)}
                sx={{ mt: -2.5 }}
              />
              <Box>
                <Typography variant="body1" fontWeight="medium">
                  Send a Scheduled Start Date Reminder
                </Typography>
                <Typography variant="body2">
                  Check if you would like to send selected users a Scheduled
                  Start Date reminder notification
                </Typography>
              </Box>
            </Box>
          </LocalizationProvider>

          {/* Start Odometer Field */}

          {/* Expected Completion Date and Actual Completion Date Row */}
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <Divider sx={{ my: 2 }} />
            <Box display="flex" gap={2} mb={0.5}>
              <Box flex={3}>
                <Typography variant="body1" fontWeight="medium">
                  Expected Completion Date
                </Typography>
              </Box>
              <Box flex={2}></Box>
              <Box flex={5}>
                <Typography variant="body1" fontWeight="medium">
                  Actual Completion Date
                </Typography>
              </Box>
            </Box>

            <Box display="flex" gap={2} alignItems="center">
              <DatePicker
                value={expectedCompletionDate}
                onChange={newValue => setExpectedCompletionDate(newValue)}
                slotProps={{ textField: { size: "small" } }}
                sx={{ flex: 3 }}
              />

              <Autocomplete
                options={timeOptions}
                value={expectedCompletionTime}
                onChange={(e, newValue) => setExpectedCompletionTime(newValue)}
                renderInput={params => (
                  <TextField
                    {...params}
                    placeholder="Select time"
                    size="small"
                  />
                )}
                sx={{ flex: 2 }}
                ListboxProps={{ style: { maxHeight: 200, overflowY: "auto" } }}
              />

              <Box sx={{ flex: 5 }} display="flex" gap={2}>
                <DatePicker
                  value={actualCompletionDate}
                  onChange={newValue => setActualCompletionDate(newValue)}
                  slotProps={{ textField: { size: "small" } }}
                  sx={{ flex: 3 }}
                />
                <Autocomplete
                  options={timeOptions}
                  value={actualCompletionTime}
                  onChange={(e, newValue) => setActualCompletionTime(newValue)}
                  renderInput={params => (
                    <TextField
                      {...params}
                      placeholder="Select time"
                      size="small"
                    />
                  )}
                  sx={{ flex: 2 }}
                  ListboxProps={{
                    style: { maxHeight: 200, overflowY: "auto" },
                  }}
                />
              </Box>
            </Box>

            {/* Use Start Odometer Checkbox */}
            <Box display="flex" alignItems="center" mt={2} mb={1}>
              <Checkbox
                checked={useStartOdometer}
                onChange={e => setUseStartOdometer(e.target.checked)}
                sx={{ mt: -2.5 }}
              />
              <Box>
                <Typography variant="body1" fontWeight="medium">
                  Use start odometer for completion meter
                </Typography>
                <Typography variant="body2">
                  Uncheck if meter usage has increased since work order start
                  date
                </Typography>
              </Box>
            </Box>
          </LocalizationProvider>

          {/* Assigned To Field */}
          <Divider sx={{ my: 2 }} />
          <Typography variant="body1" fontWeight="medium" mb={1} mt={2}>
            Assigned To
          </Typography>
          <Autocomplete
            options={assignedToOptions}
            value={assignedTo}
            onChange={(e, newValue) => setAssignedTo(newValue)}
            renderInput={params => (
              <TextField
                {...params}
                placeholder="Please select"
                size="small"
                fullWidth
              />
            )}
          />

          {/* Labels Field */}
          <Typography variant="body1" fontWeight="medium" mb={1} mt={2}>
            Labels
          </Typography>
          <Autocomplete
            options={labelOptions}
            value={labels}
            onChange={(e, newValue) => setLabels(newValue)}
            renderInput={params => (
              <TextField
                {...params}
                placeholder="Please select"
                size="small"
                fullWidth
              />
            )}
          />

          {/* Vendor Field */}
          <Typography variant="body1" fontWeight="medium" mb={1} mt={2}>
            Vendor
          </Typography>
          <Autocomplete
            options={vendorOptions}
            value={vendor}
            onChange={(e, newValue) => setVendor(newValue)}
            renderInput={params => (
              <TextField
                {...params}
                placeholder="Please select"
                size="small"
                fullWidth
              />
            )}
          />

          {/* Invoice & Po Number */}
          <Box display="flex" gap={2} mb={1} mt={2}>
            <Box flex={5}>
              <Typography variant="body1" fontWeight="medium">
                Invoice Number
              </Typography>
            </Box>
            <Box flex={5}>
              <Typography variant="body1" fontWeight="medium">
                PO Number
              </Typography>
            </Box>
          </Box>
          <Box display="flex" gap={2} alignItems="center">
            <TextField
              value={invoiceNumber}
              onChange={e => setInvoiceNumber(e.target.value)}
              fullWidth
              size="small"
              sx={{ flex: 5 }}
            />
            <TextField
              value={poNumber}
              onChange={e => setPoNumber(e.target.value)}
              fullWidth
              size="small"
              sx={{ flex: 5 }}
            />
          </Box>
        </Paper>
      </Box>
    </Box>
  );
};

export default WorkOrderHeader;

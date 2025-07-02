"use client";

import React from "react";
import { useRouter } from "next/navigation";
import {
  Box,
  Button,
  Typography,
  TextField,
  Menu,
  MenuItem,
  Paper,
  Autocomplete,
  Divider,
  Checkbox,
  Breadcrumbs,
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import Link from "next/link";
import { ChevronDown } from "lucide-react";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { DatePicker } from "@mui/x-date-pickers";
import { useWorkOrderFormStore } from "../store/workOrderFormStore";
import { useWorkOrderListStore } from "../store/workOrderListStore";

const WorkOrderHeader: React.FC = () => {
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);

  const handleMenuClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const vehicleOptions = ["1100 [2018 Toyota Prius]", "2100 [2016 Ford F-150]"];
  const statusOptions = ["Open", "Pending", "Completed"];
  const repairPriorityOptions = ["Scheduled", "Non-Scheduled", "Emergency"];

  const userOptions = ["Licht Potato", "Andy Miller", "Carlos Garcia"];
  const timeOptions = [];
  for (let hour = 0; hour < 24; hour++) {
    timeOptions.push(`${hour % 12 || 12}:00${hour < 12 ? "am" : "pm"}`);
    timeOptions.push(`${hour % 12 || 12}:30${hour < 12 ? "am" : "pm"}`);
  }

  const assignedToOptions = ["Licht Potato", "Andy Miller", "Carlos Garcia"];
  const labelOptions = ["Test"];
  const vendorOptions = ["Elite Tire and Service Inc."];

  const {
    formData,
    updateDetails,
    updateScheduling,
    updateOdometer,
    resetForm,
  } = useWorkOrderFormStore();

  const router = useRouter();
  const addWorkOrder = useWorkOrderListStore(state => state.addWorkOrder);

  const handleSaveWorkOrder = () => {
    const { vehicleId, status, repairPriorityClass } = formData.details;
    const { issueDate } = formData.scheduling;

    const missingFields = [];

    if (!vehicleId) missingFields.push("Vehicle");
    if (!status) missingFields.push("Status");
    if (!repairPriorityClass) missingFields.push("Repair Priority Class");
    if (!issueDate) missingFields.push("Issue Date");

    if (missingFields.length > 0) {
      alert(
        `Please fill out the required fields:\n- ${missingFields.join("\n- ")}`,
      );
      return;
    }

    addWorkOrder(formData);
    resetForm();

    router.push("/work-orders");
  };

  return (
    <Box>
      <Box
        sx={{
          position: "sticky",
          top: "64px",
          zIndex: 40,
          bgcolor: "#fff",
          borderBottom: "1px solid #e0e0e0",
        }}
      >
        <Breadcrumbs sx={{ mb: 1, px: 2, pt: 2 }}>
          <Link
            href="/work-orders"
            style={{
              textDecoration: "none",
              color: "#000",
              display: "flex",
              alignItems: "center",
            }}
          >
            <ArrowBackIcon
              sx={{
                mr: 0.5,
                fontSize: "0.9rem",
                verticalAlign: "middle",
                color: "text.secondary",
              }}
            />
            <Typography variant="body2" color="text.secondary">
              Work Orders
            </Typography>
          </Link>
        </Breadcrumbs>
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
              onClick={handleSaveWorkOrder}
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
            value={formData.details.vehicleId}
            onChange={(e, newValue) =>
              updateDetails({ vehicleId: newValue || "" })
            }
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
            value={formData.details.status}
            onChange={(e, newValue) =>
              updateDetails({ status: newValue || "Open" })
            }
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
            value={formData.details.repairPriorityClass}
            onChange={(e, newValue) =>
              updateDetails({ repairPriorityClass: newValue || "" })
            }
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
                  value={formData.scheduling.issueDate || null}
                  onChange={newValue =>
                    updateScheduling({ issueDate: newValue || null })
                  }
                  slotProps={{ textField: { size: "small" } }}
                  sx={{ flex: 3 }}
                />
              </LocalizationProvider>

              <Autocomplete
                options={timeOptions}
                value={formData.scheduling.issueTime}
                onChange={(e, newValue) =>
                  updateScheduling({ issueTime: newValue || "" })
                }
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
                value={formData.scheduling.issuedBy}
                onChange={(e, newValue) =>
                  updateScheduling({ issuedBy: newValue || "" })
                }
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
                value={formData.scheduling.scheduledStartDate || null}
                onChange={newValue =>
                  updateScheduling({ scheduledStartDate: newValue || null })
                }
                slotProps={{ textField: { size: "small" } }}
                sx={{ flex: 3 }}
              />

              <Autocomplete
                options={timeOptions}
                value={formData.scheduling.scheduledStartTime || ""}
                onChange={(e, newValue) =>
                  updateScheduling({ scheduledStartTime: newValue || "" })
                }
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
                  value={formData.scheduling.actualStartDate || null}
                  onChange={newValue =>
                    updateScheduling({ actualStartDate: newValue || null })
                  }
                  slotProps={{ textField: { size: "small" } }}
                  sx={{ flex: 3 }}
                />
                <Autocomplete
                  options={timeOptions}
                  value={formData.scheduling.actualStartTime || ""}
                  onChange={(e, newValue) =>
                    updateScheduling({ actualStartTime: newValue || "" })
                  }
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
                checked={formData.scheduling.sendReminder || false}
                onChange={e =>
                  updateScheduling({ sendReminder: e.target.checked })
                }
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
                value={formData.scheduling.expectedCompletionDate || null}
                onChange={newValue =>
                  updateScheduling({ expectedCompletionDate: newValue || null })
                }
                slotProps={{ textField: { size: "small" } }}
                sx={{ flex: 3 }}
              />

              <Autocomplete
                options={timeOptions}
                value={formData.scheduling.expectedCompletionTime || ""}
                onChange={(e, newValue) =>
                  updateScheduling({ expectedCompletionTime: newValue || "" })
                }
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
                  value={formData.scheduling.actualCompletionDate || null}
                  onChange={newValue =>
                    updateScheduling({ actualCompletionDate: newValue || null })
                  }
                  slotProps={{ textField: { size: "small" } }}
                  sx={{ flex: 3 }}
                />
                <Autocomplete
                  options={timeOptions}
                  value={formData.scheduling.actualCompletionTime || ""}
                  onChange={(e, newValue) =>
                    updateScheduling({ actualCompletionTime: newValue || "" })
                  }
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
                checked={formData.odometer.useStartOdometer || false}
                onChange={e =>
                  updateOdometer({ useStartOdometer: e.target.checked })
                }
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
            value={formData.details.assignedTo || ""}
            onChange={(e, newValue) =>
              updateDetails({ assignedTo: newValue || "" })
            }
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
            value={formData.details.labels || ""}
            onChange={(e, newValue) =>
              updateDetails({ labels: newValue || "" })
            }
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
            value={formData.details.vendor || ""}
            onChange={(e, newValue) =>
              updateDetails({ vendor: newValue || "" })
            }
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
              value={formData.details.invoiceNumber ?? ""}
              onChange={e =>
                updateDetails({ invoiceNumber: Number(e.target.value) || 0 })
              }
              fullWidth
              size="small"
              sx={{ flex: 5 }}
            />
            <TextField
              value={formData.details.poNumber ?? ""}
              onChange={e =>
                updateDetails({ poNumber: Number(e.target.value) || 0 })
              }
              fullWidth
              size="small"
              sx={{ flex: 5 }}
            />
          </Box>
        </Paper>
        <Paper
          elevation={1}
          sx={{ p: 2, mx: 25, backgroundColor: "#fff", mt: 4 }}
        >
          <Typography variant="h6" fontWeight="bold" mb={2}>
            Comments
          </Typography>

          <Box display="flex" gap={2} alignItems="flex-start">
            <Box
              sx={{
                width: 40,
                height: 40,
                borderRadius: "50%",
                backgroundColor: "#FF6B00",
                color: "#fff",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                fontWeight: "bold",
                fontSize: 14,
              }}
            >
              LP
            </Box>
            <TextField
              placeholder="Add an optional comment"
              fullWidth
              multiline
              minRows={3}
              size="small"
              variant="outlined"
            />
          </Box>
        </Paper>
        <Box
          mt={4}
          mb={3}
          display="flex"
          justifyContent="space-between"
          alignItems="center"
          mx={25}
          py={2}
          borderTop="1px solid #e0e0e0"
        >
          <Button color="primary" sx={{ textTransform: "none" }}>
            Cancel
          </Button>
          <Box display="flex" gap={1}>
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
                Save & Continue Editing
              </MenuItem>
              <MenuItem onClick={handleMenuClose}>Save & Add Another</MenuItem>
            </Menu>
            <Button
              variant="contained"
              sx={{ textTransform: "none", minHeight: 36 }}
              onClick={handleSaveWorkOrder}
            >
              Save Work Order
            </Button>
          </Box>
        </Box>
      </Box>
    </Box>
  );
};

export default WorkOrderHeader;

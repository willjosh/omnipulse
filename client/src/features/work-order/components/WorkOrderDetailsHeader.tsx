"use client";

import React, { useState } from "react";
import {
  Button,
  Divider,
  ListItemIcon,
  ListItemText,
  Menu,
  MenuItem,
  Box,
  Typography,
} from "@mui/material";
import { EditIcon } from "lucide-react";
import { EditLabelsPopover } from "./EditLabelsPopover";
import WatcherPanel from "./WatcherPanel";
import MoreHorizIcon from "@mui/icons-material/MoreHoriz";
import DeleteIcon from "@mui/icons-material/Delete";
import ArrowDropDownIcon from "@mui/icons-material/ArrowDropDown";
import EditOutlinedIcon from "@mui/icons-material/EditOutlined";
import WorkOrderHeader from "./WorkOrderHeader";
import { BreadcrumbItem } from "@/components/ui/Layout/Breadcrumbs";
import { WorkOrderWithLabels } from "../types/workOrderType";

interface WorkOrderDetailsHeaderProps {
  workOrder: WorkOrderWithLabels;
  onStatusChange?: (status: string) => void;
  onEdit?: () => void;
  onDelete?: () => void;
  onEditLabels?: () => void;
}

export default function WorkOrderDetailsHeader({
  workOrder,
  onStatusChange,
  onEdit,
  onDelete,
  onEditLabels,
}: WorkOrderDetailsHeaderProps) {
  const [statusMenuAnchor, setStatusMenuAnchor] = useState<null | HTMLElement>(
    null,
  );
  const [labelAnchorEl, setLabelAnchorEl] = useState<null | HTMLElement>(null);
  const [moreMenuAnchorEl, setMoreMenuAnchorEl] = useState<null | HTMLElement>(
    null,
  );

  const handleStatusMenuClick = (
    event: React.MouseEvent<HTMLButtonElement>,
  ) => {
    setStatusMenuAnchor(event.currentTarget);
  };

  const handleStatusMenuClose = () => {
    setStatusMenuAnchor(null);
  };

  const handleStatusChange = (value: string) => {
    onStatusChange?.(value);
    setStatusMenuAnchor(null);
  };

  const handleLabelClick = (event: React.MouseEvent<HTMLElement>) => {
    setLabelAnchorEl(event.currentTarget);
  };

  const handleLabelClose = () => {
    setLabelAnchorEl(null);
  };

  const handleMoreMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setMoreMenuAnchorEl(event.currentTarget);
  };

  const handleMoreMenuClose = () => {
    setMoreMenuAnchorEl(null);
  };

  const handleDeleteClick = () => {
    onDelete?.();
    handleMoreMenuClose();
  };

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Work Orders", href: "/work-orders" },
    { label: `Work Order #${workOrder.id}` },
  ];

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case "created":
      case "assigned":
        return "#12B76A"; // Green
      case "in_progress":
      case "waiting_parts":
        return "#F79009"; // Orange
      case "completed":
        return "#667085"; // Gray
      case "cancelled":
        return "#F04438"; // Red
      case "on_hold":
        return "#F79009"; // Orange
      default:
        return "#667085"; // Gray
    }
  };

  const actions = (
    <>
      <Button
        variant="outlined"
        size="small"
        onClick={handleLabelClick}
        startIcon={<EditIcon fontSize="small" />}
      >
        Edit Labels
      </Button>

      <WatcherPanel currentUser="Licht Potato" />

      <Divider orientation="vertical" flexItem sx={{ mx: 1 }} />

      <Button
        variant="outlined"
        size="small"
        sx={{
          minWidth: "32px",
          height: "32px",
          px: 1,
          borderRadius: "8px",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
        }}
        onClick={handleMoreMenuOpen}
      >
        <MoreHorizIcon fontSize="small" />
      </Button>

      <Menu
        anchorEl={moreMenuAnchorEl}
        open={Boolean(moreMenuAnchorEl)}
        onClose={handleMoreMenuClose}
        anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
        transformOrigin={{ vertical: "top", horizontal: "right" }}
        PaperProps={{
          elevation: 3,
          sx: { borderRadius: 2, mt: 1, minWidth: 180 },
        }}
      >
        <MenuItem onClick={handleDeleteClick}>
          <ListItemIcon>
            <DeleteIcon fontSize="small" />
          </ListItemIcon>
          <ListItemText>Delete</ListItemText>
        </MenuItem>
      </Menu>

      <Button
        variant="outlined"
        size="small"
        onClick={handleStatusMenuClick}
        endIcon={<ArrowDropDownIcon />}
        sx={{
          textTransform: "none",
          borderColor: "#d0d5dd",
          color: "#344054",
          px: 2,
        }}
      >
        <Box
          component="span"
          sx={{
            width: 10,
            height: 10,
            borderRadius: "50%",
            backgroundColor: getStatusColor(workOrder.statusLabel),
            display: "inline-block",
            mr: 1,
          }}
        />
        {workOrder.statusLabel}
      </Button>

      <Menu
        anchorEl={statusMenuAnchor}
        open={Boolean(statusMenuAnchor)}
        onClose={handleStatusMenuClose}
      >
        {[
          "Created",
          "Assigned",
          "In Progress",
          "Waiting for Parts",
          "Completed",
          "Cancelled",
          "On Hold",
        ].map(option => (
          <MenuItem key={option} onClick={() => handleStatusChange(option)}>
            {option}
          </MenuItem>
        ))}
      </Menu>

      <Button
        variant="outlined"
        size="small"
        startIcon={<EditOutlinedIcon />}
        sx={{ textTransform: "none" }}
        onClick={onEdit}
      >
        Edit
      </Button>
    </>
  );

  return (
    <>
      <WorkOrderHeader
        title={`Work Order #${workOrder.id}`}
        breadcrumbs={breadcrumbs}
        actions={actions}
      />

      <EditLabelsPopover anchorEl={labelAnchorEl} onClose={handleLabelClose} />
    </>
  );
}

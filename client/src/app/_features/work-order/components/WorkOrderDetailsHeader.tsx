"use client";

import {
  Box,
  Breadcrumbs,
  Button,
  Divider,
  ListItemIcon,
  ListItemText,
  Menu,
  MenuItem,
  Typography,
} from "@mui/material";
import Link from "next/link";
import { useState } from "react";
import { EditIcon } from "lucide-react";
import { EditLabelsPopover } from "./EditLabelsPopover";
import WatcherPanel from "./WatcherPanel";
import MoreHorizIcon from "@mui/icons-material/MoreHoriz";
import DeleteIcon from "@mui/icons-material/Delete";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import ArrowDropDownIcon from "@mui/icons-material/ArrowDropDown";
import EditOutlinedIcon from "@mui/icons-material/EditOutlined";

interface WorkOrderHeaderProps {
  workOrderId: number;
}

export default function WorkOrderHeader({ workOrderId }: WorkOrderHeaderProps) {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [status, setStatus] = useState("Open");
  const open = Boolean(anchorEl);

  const handleMenuClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleStatusChange = (value: string) => {
    setStatus(value);
    setAnchorEl(null);
  };

  const [labelAnchorEl, setLabelAnchorEl] = useState<null | HTMLElement>(null);

  const handleLabelClick = (event: React.MouseEvent<HTMLElement>) => {
    setLabelAnchorEl(event.currentTarget);
  };

  const handleLabelClose = () => {
    setLabelAnchorEl(null);
  };

  const [moreMenuAnchorEl, setMoreMenuAnchorEl] = useState<null | HTMLElement>(
    null,
  );
  const isMoreMenuOpen = Boolean(moreMenuAnchorEl);

  const handleMoreMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setMoreMenuAnchorEl(event.currentTarget);
  };

  const handleMoreMenuClose = () => {
    setMoreMenuAnchorEl(null);
  };

  const handleDeleteClick = () => {
    // Add your delete logic here
    handleMoreMenuClose();
  };

  return (
    <Box
      sx={{
        bgcolor: "#fff",
        borderBottom: "1px solid #e0e0e0", // subtle border
        boxShadow: "0px 2px 4px rgba(0, 0, 0, 0.06)", // soft shadow
        position: "sticky",
        top: "64px",
        zIndex: 30, // make sure it stays above
        px: 3,
        py: 2,
      }}
    >
      <Breadcrumbs>
        <Link
          href="/work-orders"
          style={{
            textDecoration: "none",
            color: "#000",
            display: "inline-flex",
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

      <Box display="flex" alignItems="center" justifyContent="space-between">
        <Box display="flex" alignItems="center" gap={2}>
          <Typography variant="h6" fontWeight="bold" ml={0.5}>
            Work Order #{workOrderId}
          </Typography>

          <Button
            variant="outlined"
            size="small"
            onClick={handleLabelClick}
            startIcon={<EditIcon fontSize="small" />}
          >
            Edit Labels
          </Button>

          <EditLabelsPopover
            anchorEl={labelAnchorEl}
            onClose={handleLabelClose}
          />
        </Box>

        <Box display="flex" alignItems="center" gap={1}>
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
            open={isMoreMenuOpen}
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
            onClick={handleMenuClick}
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
                backgroundColor:
                  status === "Open"
                    ? "#12B76A"
                    : status === "Pending"
                      ? "#F79009"
                      : "#667085",
                display: "inline-block",
                mr: 1,
              }}
            />
            {status}
          </Button>
          <Menu anchorEl={anchorEl} open={open} onClose={handleMenuClose}>
            {["Open", "Pending", "Completed"].map(option => (
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
          >
            Edit
          </Button>
        </Box>
      </Box>
    </Box>
  );
}

import {
  Avatar,
  AvatarGroup,
  Autocomplete,
  Box,
  Button,
  IconButton,
  Popover,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import PersonAddIcon from "@mui/icons-material/PersonAdd";
import { useState } from "react";

type WatcherPanelProps = { currentUser: string };

const WatcherPanel = ({ currentUser }: WatcherPanelProps) => {
  const userOptions = [
    "Licht Potato",
    "Harry Styles",
    "Tony Stark",
    "Steve Rogers",
    "Bruce Wayne",
  ];

  const [watchers, setWatchers] = useState<string[]>([currentUser]);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  const handleWatcherMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleWatcherMenuClose = () => {
    setAnchorEl(null);
  };

  const toggleWatch = () => {
    setWatchers(prev =>
      prev.includes(currentUser)
        ? prev.filter(w => w !== currentUser)
        : [...prev, currentUser],
    );
  };

  return (
    <Box display="flex" alignItems="center" gap={1}>
      <Box display="flex" alignItems="center" gap={0.75}>
        {/* Avatar Group */}
        <AvatarGroup
          max={4}
          sx={{ "& .MuiAvatar-root": { width: 32, height: 32, fontSize: 14 } }}
        >
          {watchers.map(name => (
            <Tooltip title={name} key={name}>
              <Avatar sx={{ bgcolor: "#ff6600" }}>
                {name
                  .split(" ")
                  .map(n => n[0])
                  .join("")}
              </Avatar>
            </Tooltip>
          ))}
        </AvatarGroup>

        {/* Add Watcher Button */}
        <Tooltip title="Add watchers" arrow>
          <IconButton
            size="small"
            onClick={handleWatcherMenuClick}
            sx={{
              width: 32,
              height: 32,
              backgroundColor: "#f5f5f5",
              border: "1px dashed #ccc",
              borderRadius: "50%",
              padding: 0,
              "&:hover": { backgroundColor: "#e0e0e0" },
            }}
          >
            <PersonAddIcon fontSize="small" />
          </IconButton>
        </Tooltip>
      </Box>

      {/* Popover for adding watchers */}
      <Popover
        open={Boolean(anchorEl)}
        anchorEl={anchorEl}
        onClose={handleWatcherMenuClose}
        anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
        transformOrigin={{ vertical: "top", horizontal: "left" }}
      >
        <Box sx={{ p: 2, width: 300 }}>
          <Typography fontWeight="medium" mb={1}>
            Add Watcher
          </Typography>
          <Autocomplete
            options={userOptions.filter(u => !watchers.includes(u))}
            onChange={(e, newValue) => {
              if (newValue) {
                setWatchers(prev => [...prev, newValue]);
                handleWatcherMenuClose();
              }
            }}
            renderInput={params => (
              <TextField
                {...params}
                placeholder="Search users"
                size="small"
                autoFocus
              />
            )}
          />
        </Box>
      </Popover>

      {/* Watch/Unwatch button */}
      <Button variant="outlined" size="small" onClick={toggleWatch}>
        {watchers.includes(currentUser) ? "Unwatch" : "Watch"}
      </Button>
    </Box>
  );
};

export default WatcherPanel;

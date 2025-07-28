import {
  Box,
  Button,
  Popover,
  TextField,
  Typography,
  Autocomplete,
} from "@mui/material";
import { useState } from "react";

interface EditLabelsPopoverProps {
  anchorEl: HTMLElement | null;
  onClose: () => void;
}

const labelOptions = ["Urgent", "Internal", "Test"];

export function EditLabelsPopover({
  anchorEl,
  onClose,
}: EditLabelsPopoverProps) {
  const open = Boolean(anchorEl);
  const [labels, setLabels] = useState<string[]>([]);
  const [inputValue, setInputValue] = useState("");

  const handleSave = () => {
    // Save logic goes here
    onClose();
  };

  return (
    <Popover
      open={open}
      anchorEl={anchorEl}
      onClose={onClose}
      anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
      PaperProps={{ sx: { width: 400, p: 2 } }}
    >
      <Typography variant="body2" mb={1}>
        Select or create labels to add to this record.
      </Typography>

      <Autocomplete
        multiple
        freeSolo
        options={labelOptions}
        value={labels}
        onChange={(event, newValue) => setLabels(newValue)}
        inputValue={inputValue}
        onInputChange={(event, newInputValue) => setInputValue(newInputValue)}
        renderInput={params => (
          <TextField
            {...params}
            size="small"
            placeholder=""
            sx={{ borderColor: "green" }}
          />
        )}
        noOptionsText="No options"
      />

      <Box mt={2} display="flex" justifyContent="flex-end" gap={1}>
        <Button onClick={onClose} variant="text" color="inherit">
          Cancel
        </Button>
        <Button
          onClick={handleSave}
          variant="contained"
          disabled={labels.length === 0}
        >
          Save
        </Button>
      </Box>
    </Popover>
  );
}

export function getTimeOptions(): string[] {
  const options: string[] = [];
  for (let hour = 0; hour < 24; hour++) {
    options.push(`${hour % 12 || 12}:00${hour < 12 ? "am" : "pm"}`);
    options.push(`${hour % 12 || 12}:30${hour < 12 ? "am" : "pm"}`);
  }
  return options;
}

// Helper to combine date and time into ISO string
export function combineDateAndTime(dateStr: string, timeStr: string): string {
  if (!dateStr || !timeStr) return "";

  try {
    const date = new Date(dateStr);
    if (isNaN(date.getTime())) return "";

    const [time, meridian] = timeStr.split(/(am|pm)/);
    let [hours, minutes] = time.split(":").map(Number);
    if (meridian === "pm" && hours !== 12) hours += 12;
    if (meridian === "am" && hours === 12) hours = 0;

    // Use UTC methods to maintain consistency with extractTimeFromISO
    date.setUTCHours(hours, minutes, 0, 0);
    return date.toISOString().replace(/\.\d{3}Z$/, "Z");
  } catch {
    return "";
  }
}

export function toISOorNull(dateStr: string | undefined): string | null {
  if (!dateStr || dateStr.trim() === "") return null;

  // Handle timestamp strings (numbers as strings)
  if (/^\d+$/.test(dateStr.trim())) {
    const timestamp = parseInt(dateStr.trim());
    const d = new Date(timestamp);
    return isNaN(d.getTime())
      ? null
      : d.toISOString().replace(/\.\d{3}Z$/, "Z");
  }

  const d = new Date(dateStr);
  if (isNaN(d.getTime())) return null;

  // Check for date auto-correction (e.g., 2023-02-29 -> 2023-03-01)
  const originalDateMatch = dateStr.match(/^(\d{4})-(\d{2})-(\d{2})/);
  if (originalDateMatch) {
    const [, year, month, day] = originalDateMatch;
    if (
      d.getFullYear() !== parseInt(year) ||
      d.getMonth() + 1 !== parseInt(month) ||
      d.getDate() !== parseInt(day)
    ) {
      return null; // Date was auto-corrected, so it was invalid
    }
  }

  return d.toISOString().replace(/\.\d{3}Z$/, "Z");
}

export function extractTimeFromISO(
  isoString: string | null | undefined,
): string {
  if (!isoString) return "";
  const date = new Date(isoString);
  if (isNaN(date.getTime())) return "";

  // Use UTC methods to maintain consistency with ISO string timezone
  const hours = date.getUTCHours();
  const minutes = date.getUTCMinutes();

  // Convert to 12-hour format
  const displayHours = hours % 12 || 12;
  const ampm = hours < 12 ? "am" : "pm";

  return `${displayHours}:${minutes.toString().padStart(2, "0")}${ampm}`;
}

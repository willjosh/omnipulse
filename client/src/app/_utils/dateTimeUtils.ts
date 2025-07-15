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
  const date = new Date(dateStr);
  const [time, meridian] = timeStr.split(/(am|pm)/);
  let [hours, minutes] = time.split(":").map(Number);
  if (meridian === "pm" && hours !== 12) hours += 12;
  if (meridian === "am" && hours === 12) hours = 0;
  date.setHours(hours, minutes, 0, 0);
  return date.toISOString();
}

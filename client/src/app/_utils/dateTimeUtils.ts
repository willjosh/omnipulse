export function getTimeOptions(): string[] {
  const options: string[] = [];
  for (let hour = 0; hour < 24; hour++) {
    options.push(`${hour % 12 || 12}:00${hour < 12 ? "am" : "pm"}`);
    options.push(`${hour % 12 || 12}:30${hour < 12 ? "am" : "pm"}`);
  }
  return options;
}

export function getErrorFields(
  error: any,
  fieldsToHighlight?: string[],
): Record<string, string> {
  const fieldErrors: Record<string, string> = {};

  const structuredErrors = error?.response?.data?.errors;

  if (structuredErrors && typeof structuredErrors === "object") {
    for (const [fieldName, messages] of Object.entries(structuredErrors)) {
      if (Array.isArray(messages) && messages.length > 0) {
        fieldErrors[fieldName] = "error";
      } else if (typeof messages === "string") {
        fieldErrors[fieldName] = "error";
      }
    }
  } else {
    const errorMessage =
      error?.response?.data?.detail || error?.response?.data?.error;
    if (errorMessage && typeof errorMessage === "string" && fieldsToHighlight) {
      fieldsToHighlight.forEach(fieldName => {
        const variations = [
          fieldName.toLowerCase(),
          fieldName
            .replace(/([A-Z])/g, " $1")
            .toLowerCase()
            .trim(),
          fieldName.replace(/([A-Z])/g, " $1").trim(),
        ];

        if (
          variations.some(variation =>
            errorMessage.toLowerCase().includes(variation),
          )
        ) {
          fieldErrors[fieldName] = "error";
        }
      });
    }
  }

  return fieldErrors;
}

export function getErrorMessage(error: any, backupMessage: string): string {
  if (error?.response?.data) {
    return (
      error.response.data?.detail || error.response.data?.error || backupMessage
    );
  }
  return backupMessage;
}

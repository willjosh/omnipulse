import { getErrorFields, getErrorMessage } from "@/utils/fieldErrorUtils";

describe("fieldErrorUtils", () => {
  describe("getErrorFields", () => {
    test("should extract structured errors from response", () => {
      const error = {
        response: {
          data: {
            errors: {
              firstName: ["First name is required"],
              email: ["Email is invalid", "Email already exists"],
              password: "Password is too short",
            },
          },
        },
      };

      const result = getErrorFields(error);

      expect(result).toEqual({
        firstName: "error",
        email: "error",
        password: "error",
      });
    });

    test("should handle array error messages", () => {
      const error = {
        response: {
          data: {
            errors: {
              username: ["Username is required", "Username must be unique"],
              age: [],
            },
          },
        },
      };

      const result = getErrorFields(error);

      expect(result).toEqual({ username: "error" });
    });

    test("should handle string error messages", () => {
      const error = {
        response: {
          data: {
            errors: {
              name: "Name is required",
              description: "Description is too long",
            },
          },
        },
      };

      const result = getErrorFields(error);

      expect(result).toEqual({ name: "error", description: "error" });
    });

    test("should handle non-structured errors with field highlighting", () => {
      const error = {
        response: {
          data: {
            detail: "The email field is invalid and the password is too short",
          },
        },
      };

      const fieldsToHighlight = ["email", "password", "firstName"];
      const result = getErrorFields(error, fieldsToHighlight);

      expect(result).toEqual({ email: "error", password: "error" });
    });

    test("should handle camelCase field matching", () => {
      const error = {
        response: {
          data: {
            detail: "The first name is required and last name cannot be empty",
          },
        },
      };

      const fieldsToHighlight = ["firstName", "lastName", "email"];
      const result = getErrorFields(error, fieldsToHighlight);

      expect(result).toEqual({ firstName: "error", lastName: "error" });
    });

    test("should handle case-insensitive matching", () => {
      const error = {
        response: { data: { detail: "EMAIL ADDRESS IS INVALID" } },
      };

      const fieldsToHighlight = ["emailAddress"];
      const result = getErrorFields(error, fieldsToHighlight);

      expect(result).toEqual({ emailAddress: "error" });
    });

    test("should return empty object when no errors", () => {
      const error = { response: { data: { errors: {} } } };

      const result = getErrorFields(error);

      expect(result).toEqual({});
    });

    test("should return empty object when no structured errors", () => {
      const error = { response: { data: { message: "Something went wrong" } } };

      const result = getErrorFields(error);

      expect(result).toEqual({});
    });

    test("should handle missing response data", () => {
      const error = { response: null };

      const result = getErrorFields(error);

      expect(result).toEqual({});
    });

    test("should handle undefined error", () => {
      const result = getErrorFields(undefined);

      expect(result).toEqual({});
    });

    test("should handle null error", () => {
      const result = getErrorFields(null);

      expect(result).toEqual({});
    });

    test("should handle error without response", () => {
      const error = { message: "Network error" };

      const result = getErrorFields(error);

      expect(result).toEqual({});
    });

    test("should handle complex field name variations", () => {
      const error = {
        response: {
          data: {
            detail:
              "vehicle group id is invalid and assigned technician name is missing",
          },
        },
      };

      const fieldsToHighlight = ["vehicleGroupId", "assignedTechnicianName"];
      const result = getErrorFields(error, fieldsToHighlight);

      expect(result).toEqual({
        vehicleGroupId: "error",
        assignedTechnicianName: "error",
      });
    });

    test("should not match partial field names", () => {
      const error = {
        response: { data: { detail: "The name field is invalid" } },
      };

      const fieldsToHighlight = ["firstName", "lastName"];
      const result = getErrorFields(error, fieldsToHighlight);

      expect(result).toEqual({});
    });

    test("should handle error with both structured and detail message", () => {
      const error = {
        response: {
          data: {
            errors: { email: ["Email is required"] },
            detail: "Additional error information",
          },
        },
      };

      const result = getErrorFields(error);

      expect(result).toEqual({ email: "error" });
    });

    test("should prioritize structured errors over detail message", () => {
      const error = {
        response: {
          data: {
            errors: { username: ["Username is taken"] },
            detail: "The password field is invalid",
          },
        },
      };

      const fieldsToHighlight = ["username", "password"];
      const result = getErrorFields(error, fieldsToHighlight);

      expect(result).toEqual({ username: "error" });
    });
  });

  describe("getErrorMessage", () => {
    test("should extract detail message from response", () => {
      const error = { response: { data: { detail: "Validation failed" } } };

      const result = getErrorMessage(error, "Default message");

      expect(result).toBe("Validation failed");
    });

    test("should extract error message from response", () => {
      const error = { response: { data: { error: "Authentication failed" } } };

      const result = getErrorMessage(error, "Default message");

      expect(result).toBe("Authentication failed");
    });

    test("should prioritize detail over error field", () => {
      const error = {
        response: {
          data: {
            detail: "Detailed error message",
            error: "Generic error message",
          },
        },
      };

      const result = getErrorMessage(error, "Default message");

      expect(result).toBe("Detailed error message");
    });

    test("should return backup message when no error data", () => {
      const error = { response: { data: { message: "Some other message" } } };

      const result = getErrorMessage(error, "Backup message");

      expect(result).toBe("Backup message");
    });

    test("should return backup message when no response", () => {
      const error = { message: "Network error" };

      const result = getErrorMessage(error, "Connection failed");

      expect(result).toBe("Connection failed");
    });

    test("should return backup message for undefined error", () => {
      const result = getErrorMessage(undefined, "Something went wrong");

      expect(result).toBe("Something went wrong");
    });

    test("should return backup message for null error", () => {
      const result = getErrorMessage(null, "Unknown error");

      expect(result).toBe("Unknown error");
    });

    test("should handle empty response data", () => {
      const error = { response: { data: {} } };

      const result = getErrorMessage(error, "No error details");

      expect(result).toBe("No error details");
    });

    test("should handle null response data", () => {
      const error = { response: { data: null } };

      const result = getErrorMessage(error, "No data available");

      expect(result).toBe("No data available");
    });

    test("should handle missing response.data", () => {
      const error = { response: {} };

      const result = getErrorMessage(error, "Response error");

      expect(result).toBe("Response error");
    });

    test("should handle complex error structures", () => {
      const error = {
        response: {
          data: { detail: "", error: "", message: "This should not be used" },
        },
      };

      const result = getErrorMessage(error, "Fallback message");

      expect(result).toBe("Fallback message");
    });

    test("should handle whitespace-only error messages", () => {
      const error = { response: { data: { detail: "   ", error: "\t\n" } } };

      const result = getErrorMessage(error, "Clean message");

      expect(result).toBe("Clean message");
    });
  });

  describe("Integration scenarios", () => {
    test("should work together for API validation errors", () => {
      const apiError = {
        response: {
          data: {
            errors: {
              email: ["Email is required"],
              password: ["Password must be at least 8 characters"],
            },
            detail: "Validation failed for multiple fields",
          },
        },
      };

      const fieldErrors = getErrorFields(apiError);
      const errorMessage = getErrorMessage(apiError, "Form submission failed");

      expect(fieldErrors).toEqual({ email: "error", password: "error" });
      expect(errorMessage).toBe("Validation failed for multiple fields");
    });

    test("should handle network errors gracefully", () => {
      const networkError = { message: "Network Error", code: "NETWORK_ERROR" };

      const fieldErrors = getErrorFields(networkError, ["email", "password"]);
      const errorMessage = getErrorMessage(networkError, "Connection lost");

      expect(fieldErrors).toEqual({});
      expect(errorMessage).toBe("Connection lost");
    });

    test("should handle server 500 errors", () => {
      const serverError = {
        response: {
          data: { detail: "Internal server error occurred" },
          status: 500,
        },
      };

      const fieldErrors = getErrorFields(serverError);
      const errorMessage = getErrorMessage(serverError, "Server unavailable");

      expect(fieldErrors).toEqual({});
      expect(errorMessage).toBe("Internal server error occurred");
    });
  });
});

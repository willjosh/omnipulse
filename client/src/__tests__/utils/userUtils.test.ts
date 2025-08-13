import {
  getUserInitials,
  getUserDisplayName,
  getFormattedRole,
} from "@/utils/userUtils";
import { AuthUser } from "@/features/auth/types/authType";

describe("userUtils", () => {
  describe("getUserInitials", () => {
    test("should return initials from first and last name", () => {
      const result = getUserInitials("John", "Doe");
      expect(result).toBe("JD");
    });

    test("should handle lowercase names", () => {
      const result = getUserInitials("jane", "smith");
      expect(result).toBe("JS");
    });

    test("should handle mixed case names", () => {
      const result = getUserInitials("miChAeL", "jOhNsOn");
      expect(result).toBe("MJ");
    });

    test("should handle single character names", () => {
      const result = getUserInitials("A", "B");
      expect(result).toBe("AB");
    });

    test("should handle names with spaces", () => {
      const result = getUserInitials("Mary Jane", "Watson");
      expect(result).toBe("MW");
    });

    test("should handle names with special characters", () => {
      const result = getUserInitials("José", "García");
      expect(result).toBe("JG");
    });

    test("should return NN when firstName is missing", () => {
      const result = getUserInitials(undefined, "Doe");
      expect(result).toBe("NN");
    });

    test("should return NN when lastName is missing", () => {
      const result = getUserInitials("John", undefined);
      expect(result).toBe("NN");
    });

    test("should return NN when both names are missing", () => {
      const result = getUserInitials(undefined, undefined);
      expect(result).toBe("NN");
    });

    test("should return NN when firstName is empty string", () => {
      const result = getUserInitials("", "Doe");
      expect(result).toBe("NN");
    });

    test("should return NN when lastName is empty string", () => {
      const result = getUserInitials("John", "");
      expect(result).toBe("NN");
    });

    test("should return NN when both names are empty strings", () => {
      const result = getUserInitials("", "");
      expect(result).toBe("NN");
    });

    test("should return NN when firstName is whitespace only", () => {
      const result = getUserInitials("   ", "Doe");
      expect(result).toBe("NN");
    });

    test("should return NN when lastName is whitespace only", () => {
      const result = getUserInitials("John", "   ");
      expect(result).toBe("NN");
    });

    test("should handle very long names", () => {
      const result = getUserInitials("Christopher", "Williamson");
      expect(result).toBe("CW");
    });

    test("should handle unicode characters", () => {
      const result = getUserInitials("李", "王");
      expect(result).toBe("李王");
    });

    test("should handle numbers in names", () => {
      const result = getUserInitials("John2", "Doe3");
      expect(result).toBe("JD");
    });
  });

  describe("getUserDisplayName", () => {
    const createUser = (firstName?: string, lastName?: string): AuthUser => ({
      id: "1",
      firstName: firstName || "",
      lastName: lastName || "",
      email: "test@example.com",
      role: "Technician",
    });

    test("should return full name for valid user", () => {
      const user = createUser("John", "Doe");
      const result = getUserDisplayName(user);
      expect(result).toBe("John Doe");
    });

    test("should handle user with middle names", () => {
      const user = createUser("Mary Jane", "Watson");
      const result = getUserDisplayName(user);
      expect(result).toBe("Mary Jane Watson");
    });

    test("should handle names with special characters", () => {
      const user = createUser("José", "García-López");
      const result = getUserDisplayName(user);
      expect(result).toBe("José García-López");
    });

    test("should return No Name when user is null", () => {
      const result = getUserDisplayName(null);
      expect(result).toBe("No Name");
    });

    test("should return No Name when user is undefined", () => {
      const result = getUserDisplayName(undefined);
      expect(result).toBe("No Name");
    });

    test("should return No Name when firstName is missing", () => {
      const user = createUser(undefined, "Doe");
      const result = getUserDisplayName(user);
      expect(result).toBe("No Name");
    });

    test("should return No Name when lastName is missing", () => {
      const user = createUser("John", undefined);
      const result = getUserDisplayName(user);
      expect(result).toBe("No Name");
    });

    test("should return No Name when both names are missing", () => {
      const user = createUser(undefined, undefined);
      const result = getUserDisplayName(user);
      expect(result).toBe("No Name");
    });

    test("should return No Name when firstName is empty string", () => {
      const user = createUser("", "Doe");
      const result = getUserDisplayName(user);
      expect(result).toBe("No Name");
    });

    test("should return No Name when lastName is empty string", () => {
      const user = createUser("John", "");
      const result = getUserDisplayName(user);
      expect(result).toBe("No Name");
    });

    test("should return No Name when both names are empty strings", () => {
      const user = createUser("", "");
      const result = getUserDisplayName(user);
      expect(result).toBe("No Name");
    });

    test("should handle names with extra whitespace", () => {
      const user = createUser("  John  ", "  Doe  ");
      const result = getUserDisplayName(user);
      expect(result).toBe("  John     Doe  ");
    });

    test("should handle single character names", () => {
      const user = createUser("A", "B");
      const result = getUserDisplayName(user);
      expect(result).toBe("A B");
    });

    test("should handle very long names", () => {
      const user = createUser("Christopher Alexander", "Williamson-Fitzgerald");
      const result = getUserDisplayName(user);
      expect(result).toBe("Christopher Alexander Williamson-Fitzgerald");
    });

    test("should handle unicode characters", () => {
      const user = createUser("李明", "王小红");
      const result = getUserDisplayName(user);
      expect(result).toBe("李明 王小红");
    });
  });

  describe("getFormattedRole", () => {
    const createUser = (role?: string): AuthUser => ({
      id: "1",
      firstName: "John",
      lastName: "Doe",
      email: "test@example.com",
      role: role as any,
    });

    test("should format FleetManager role", () => {
      const user = createUser("FleetManager");
      const result = getFormattedRole(user);
      expect(result).toBe("Fleet Manager");
    });

    test("should return Technician role as-is", () => {
      const user = createUser("Technician");
      const result = getFormattedRole(user);
      expect(result).toBe("Technician");
    });

    test("should handle other roles as-is", () => {
      const user = createUser("Admin");
      const result = getFormattedRole(user);
      expect(result).toBe("Admin");
    });

    test("should return No Role when user is null", () => {
      const result = getFormattedRole(null);
      expect(result).toBe("No Role");
    });

    test("should return No Role when user is undefined", () => {
      const result = getFormattedRole(undefined);
      expect(result).toBe("No Role");
    });

    test("should return No Role when role is missing", () => {
      const user = createUser(undefined);
      const result = getFormattedRole(user);
      expect(result).toBe("No Role");
    });

    test("should return No Role when role is empty string", () => {
      const user = createUser(undefined);
      user.role = "" as any;
      const result = getFormattedRole(user);
      expect(result).toBe("No Role");
    });

    test("should handle case sensitivity for FleetManager", () => {
      const user = createUser("fleetmanager");
      const result = getFormattedRole(user);
      expect(result).toBe("fleetmanager");
    });

    test("should handle FleetManager with different casing", () => {
      const user = createUser("FLEETMANAGER");
      const result = getFormattedRole(user);
      expect(result).toBe("FLEETMANAGER");
    });

    test("should handle mixed case roles", () => {
      const user = createUser("SuperAdmin");
      const result = getFormattedRole(user);
      expect(result).toBe("SuperAdmin");
    });

    test("should handle roles with spaces", () => {
      const user = createUser("Fleet Manager");
      const result = getFormattedRole(user);
      expect(result).toBe("Fleet Manager");
    });

    test("should handle roles with special characters", () => {
      const user = createUser("Tech-Lead");
      const result = getFormattedRole(user);
      expect(result).toBe("Tech-Lead");
    });

    test("should handle numeric roles", () => {
      const user = createUser("Level1");
      const result = getFormattedRole(user);
      expect(result).toBe("Level1");
    });
  });

  describe("Integration scenarios", () => {
    test("should work together for complete user display", () => {
      const user: AuthUser = {
        id: "1",
        firstName: "John",
        lastName: "Doe",
        email: "john.doe@example.com",
        role: "FleetManager",
      };

      const initials = getUserInitials(user.firstName, user.lastName);
      const displayName = getUserDisplayName(user);
      const formattedRole = getFormattedRole(user);

      expect(initials).toBe("JD");
      expect(displayName).toBe("John Doe");
      expect(formattedRole).toBe("Fleet Manager");
    });

    test("should handle incomplete user data gracefully", () => {
      const incompleteUser: Partial<AuthUser> = {
        id: "1",
        email: "user@example.com",
      };

      const initials = getUserInitials(
        incompleteUser.firstName,
        incompleteUser.lastName,
      );
      const displayName = getUserDisplayName(incompleteUser as AuthUser);
      const formattedRole = getFormattedRole(incompleteUser as AuthUser);

      expect(initials).toBe("NN");
      expect(displayName).toBe("No Name");
      expect(formattedRole).toBe("No Role");
    });

    test("should handle edge case user data", () => {
      const edgeUser: AuthUser = {
        id: "1",
        firstName: "   ",
        lastName: "",
        email: "edge@example.com",
        role: "" as any,
      };

      const initials = getUserInitials(edgeUser.firstName, edgeUser.lastName);
      const displayName = getUserDisplayName(edgeUser);
      const formattedRole = getFormattedRole(edgeUser);

      expect(initials).toBe("NN");
      expect(displayName).toBe("No Name");
      expect(formattedRole).toBe("No Role");
    });

    test("should handle technician user properly", () => {
      const techUser: AuthUser = {
        id: "2",
        firstName: "Jane",
        lastName: "Smith",
        email: "jane.smith@example.com",
        role: "Technician",
      };

      const initials = getUserInitials(techUser.firstName, techUser.lastName);
      const displayName = getUserDisplayName(techUser);
      const formattedRole = getFormattedRole(techUser);

      expect(initials).toBe("JS");
      expect(displayName).toBe("Jane Smith");
      expect(formattedRole).toBe("Technician");
    });
  });
});

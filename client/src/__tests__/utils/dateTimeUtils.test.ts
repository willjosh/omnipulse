import {
  getTimeOptions,
  combineDateAndTime,
  toISOorNull,
  extractTimeFromISO,
} from "@/utils/dateTimeUtils";

describe("dateTimeUtils", () => {
  describe("getTimeOptions", () => {
    test("should return 48 time options (24 hours × 2 intervals)", () => {
      const options = getTimeOptions();
      expect(options).toHaveLength(48);
    });

    test("should include midnight options", () => {
      const options = getTimeOptions();
      expect(options).toContain("12:00am");
      expect(options).toContain("12:30am");
    });

    test("should include noon options", () => {
      const options = getTimeOptions();
      expect(options).toContain("12:00pm");
      expect(options).toContain("12:30pm");
    });

    test("should include early morning options", () => {
      const options = getTimeOptions();
      expect(options).toContain("1:00am");
      expect(options).toContain("1:30am");
    });

    test("should include evening options", () => {
      const options = getTimeOptions();
      expect(options).toContain("11:00pm");
      expect(options).toContain("11:30pm");
    });

    test("should format single digit hours correctly", () => {
      const options = getTimeOptions();
      expect(options).toContain("9:00am");
      expect(options).toContain("9:30am");
      expect(options).toContain("9:00pm");
      expect(options).toContain("9:30pm");
    });

    test("should have proper chronological order for morning hours", () => {
      const options = getTimeOptions();
      const midnightIndex = options.indexOf("12:00am");
      const oneAmIndex = options.indexOf("1:00am");
      const twoAmIndex = options.indexOf("2:00am");

      expect(midnightIndex).toBeLessThan(oneAmIndex);
      expect(oneAmIndex).toBeLessThan(twoAmIndex);
    });

    test("should have proper chronological order for afternoon hours", () => {
      const options = getTimeOptions();
      const noonIndex = options.indexOf("12:00pm");
      const onePmIndex = options.indexOf("1:00pm");
      const twoPmIndex = options.indexOf("2:00pm");

      expect(noonIndex).toBeLessThan(onePmIndex);
      expect(onePmIndex).toBeLessThan(twoPmIndex);
    });

    test("should return same array on multiple calls", () => {
      const options1 = getTimeOptions();
      const options2 = getTimeOptions();
      expect(options1).toEqual(options2);
    });

    test("should include all expected time intervals", () => {
      const options = getTimeOptions();
      const expectedTimes = [
        "12:00am",
        "12:30am",
        "1:00am",
        "1:30am",
        "6:00am",
        "6:30am",
        "12:00pm",
        "12:30pm",
        "6:00pm",
        "6:30pm",
        "11:00pm",
        "11:30pm",
      ];

      expectedTimes.forEach(time => {
        expect(options).toContain(time);
      });
    });
  });

  describe("combineDateAndTime", () => {
    test("should combine date and AM time correctly", () => {
      const result = combineDateAndTime("2023-12-25", "9:30am");
      const date = new Date(result);

      expect(date.getFullYear()).toBe(2023);
      expect(date.getMonth()).toBe(11);
      expect(date.getDate()).toBe(25);
      expect(date.getUTCHours()).toBe(9);
      expect(date.getUTCMinutes()).toBe(30);
    });

    test("should combine date and PM time correctly", () => {
      const result = combineDateAndTime("2023-12-25", "2:45pm");
      const date = new Date(result);

      expect(date.getUTCHours()).toBe(14);
      expect(date.getUTCMinutes()).toBe(45);
    });

    test("should handle midnight (12:00am) correctly", () => {
      const result = combineDateAndTime("2023-12-25", "12:00am");
      const date = new Date(result);

      expect(date.getUTCHours()).toBe(0);
      expect(date.getUTCMinutes()).toBe(0);
    });

    test("should handle noon (12:00pm) correctly", () => {
      const result = combineDateAndTime("2023-12-25", "12:00pm");
      const date = new Date(result);

      expect(date.getUTCHours()).toBe(12);
      expect(date.getUTCMinutes()).toBe(0);
    });

    test("should handle 12:30am correctly", () => {
      const result = combineDateAndTime("2023-12-25", "12:30am");
      const date = new Date(result);

      expect(date.getUTCHours()).toBe(0);
      expect(date.getUTCMinutes()).toBe(30);
    });

    test("should handle 12:30pm correctly", () => {
      const result = combineDateAndTime("2023-12-25", "12:30pm");
      const date = new Date(result);

      expect(date.getUTCHours()).toBe(12);
      expect(date.getUTCMinutes()).toBe(30);
    });

    test("should return empty string when date is missing", () => {
      const result = combineDateAndTime("", "9:30am");
      expect(result).toBe("");
    });

    test("should return empty string when time is missing", () => {
      const result = combineDateAndTime("2023-12-25", "");
      expect(result).toBe("");
    });

    test("should return empty string when both are missing", () => {
      const result = combineDateAndTime("", "");
      expect(result).toBe("");
    });

    test("should handle different date formats", () => {
      const result = combineDateAndTime("2023/12/25", "3:15pm");
      const date = new Date(result);

      expect(date.getFullYear()).toBe(2023);
      expect(date.getMonth()).toBe(11);
      expect(date.getDate()).toBe(25);
      expect(date.getUTCHours()).toBe(15);
      expect(date.getUTCMinutes()).toBe(15);
    });

    test("should return ISO string format without milliseconds", () => {
      const result = combineDateAndTime("2023-12-25", "9:30am");
      expect(result).toMatch(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$/);
      expect(result).not.toContain(".");
    });

    test("should set seconds and milliseconds to zero", () => {
      const result = combineDateAndTime("2023-12-25", "9:30am");
      const date = new Date(result);

      expect(date.getSeconds()).toBe(0);
      expect(date.getMilliseconds()).toBe(0);
    });

    test("should handle edge case times", () => {
      const result1 = combineDateAndTime("2023-12-25", "11:59pm");
      const date1 = new Date(result1);
      expect(date1.getUTCHours()).toBe(23);
      expect(date1.getUTCMinutes()).toBe(59);

      const result2 = combineDateAndTime("2023-12-25", "1:01am");
      const date2 = new Date(result2);
      expect(date2.getUTCHours()).toBe(1);
      expect(date2.getUTCMinutes()).toBe(1);
    });
  });

  describe("toISOorNull", () => {
    test("should convert valid date string to ISO", () => {
      const result = toISOorNull("2023-12-25");
      expect(result).toMatch(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$/);
    });

    test("should return null for undefined input", () => {
      const result = toISOorNull(undefined);
      expect(result).toBeNull();
    });

    test("should return null for empty string", () => {
      const result = toISOorNull("");
      expect(result).toBeNull();
    });

    test("should return null for invalid date string", () => {
      const result = toISOorNull("invalid-date");
      expect(result).toBeNull();
    });

    test("should handle various valid date formats", () => {
      const validDates = [
        "2023-12-25",
        "2023/12/25",
        "Dec 25, 2023",
        "2023-12-25T10:30:00Z",
      ];

      validDates.forEach(dateStr => {
        const result = toISOorNull(dateStr);
        expect(result).not.toBeNull();
        expect(result).toMatch(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$/);
      });
    });

    test("should remove milliseconds from ISO string", () => {
      const result = toISOorNull("2023-12-25T10:30:00.123Z");
      expect(result).not.toContain(".");
      expect(result).toMatch(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$/);
    });

    test("should handle timestamp numbers as strings", () => {
      const timestamp = Date.now().toString();
      const result = toISOorNull(timestamp);
      expect(result).not.toBeNull();
      expect(result).toMatch(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$/);
    });

    test("should return null for whitespace-only string", () => {
      const result = toISOorNull("   ");
      expect(result).toBeNull();
    });

    test("should handle leap year dates", () => {
      const result = toISOorNull("2024-02-29"); // Leap year
      expect(result).not.toBeNull();
      expect(result).toMatch(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$/);
    });

    test("should return null for invalid leap year date", () => {
      const result = toISOorNull("2023-02-29"); // Not a leap year
      expect(result).toBeNull();
    });
  });

  describe("extractTimeFromISO", () => {
    test("should extract AM time correctly", () => {
      const isoString = "2023-12-25T09:30:00Z";
      const result = extractTimeFromISO(isoString);
      expect(result).toBe("9:30am");
    });

    test("should extract PM time correctly", () => {
      const isoString = "2023-12-25T14:45:00Z";
      const result = extractTimeFromISO(isoString);
      expect(result).toBe("2:45pm");
    });

    test("should handle midnight correctly", () => {
      const isoString = "2023-12-25T00:00:00Z";
      const result = extractTimeFromISO(isoString);
      expect(result).toBe("12:00am");
    });

    test("should handle noon correctly", () => {
      const isoString = "2023-12-25T12:00:00Z";
      const result = extractTimeFromISO(isoString);
      expect(result).toBe("12:00pm");
    });

    test("should handle 12:30 AM correctly", () => {
      const isoString = "2023-12-25T00:30:00Z";
      const result = extractTimeFromISO(isoString);
      expect(result).toBe("12:30am");
    });

    test("should handle 12:30 PM correctly", () => {
      const isoString = "2023-12-25T12:30:00Z";
      const result = extractTimeFromISO(isoString);
      expect(result).toBe("12:30pm");
    });

    test("should pad single digit minutes", () => {
      const isoString = "2023-12-25T09:05:00Z";
      const result = extractTimeFromISO(isoString);
      expect(result).toBe("9:05am");
    });

    test("should return empty string for null input", () => {
      const result = extractTimeFromISO(null);
      expect(result).toBe("");
    });

    test("should return empty string for undefined input", () => {
      const result = extractTimeFromISO(undefined);
      expect(result).toBe("");
    });

    test("should return empty string for empty string input", () => {
      const result = extractTimeFromISO("");
      expect(result).toBe("");
    });

    test("should return empty string for invalid ISO string", () => {
      const result = extractTimeFromISO("invalid-iso-string");
      expect(result).toBe("");
    });

    test("should handle different ISO string formats", () => {
      const isoStrings = [
        "2023-12-25T15:30:00Z",
        "2023-12-25T15:30:00.000Z",
        "2023-12-25T15:30:00+00:00",
      ];

      isoStrings.forEach(isoString => {
        const result = extractTimeFromISO(isoString);
        expect(result).toBe("3:30pm");
      });
    });

    test("should handle timezone variations", () => {
      // Note: This test assumes the function works with UTC time
      const isoString = "2023-12-25T15:30:00-05:00";
      const result = extractTimeFromISO(isoString);
      expect(result).toMatch(/^\d{1,2}:\d{2}(am|pm)$/);
    });

    test("should handle edge case times", () => {
      const testCases = [
        { iso: "2023-12-25T23:59:00Z", expected: "11:59pm" },
        { iso: "2023-12-25T01:01:00Z", expected: "1:01am" },
        { iso: "2023-12-25T13:00:00Z", expected: "1:00pm" },
        { iso: "2023-12-25T11:00:00Z", expected: "11:00am" },
      ];

      testCases.forEach(({ iso, expected }) => {
        const result = extractTimeFromISO(iso);
        expect(result).toBe(expected);
      });
    });
  });

  describe("Integration scenarios", () => {
    test("should work together for round-trip conversion", () => {
      const originalDate = "2023-12-25";
      const originalTime = "2:30pm";

      // Combine date and time
      const isoString = combineDateAndTime(originalDate, originalTime);

      // Extract time back
      const extractedTime = extractTimeFromISO(isoString);

      expect(extractedTime).toBe(originalTime);
    });

    test("should handle complete workflow", () => {
      const dateStr = "2023-12-25";
      const timeStr = "9:15am";

      // Combine into ISO
      const combined = combineDateAndTime(dateStr, timeStr);
      expect(combined).toBeTruthy();

      // Convert to ISO or null
      const iso = toISOorNull(combined);
      expect(iso).toBe(combined);

      // Extract time
      const extracted = extractTimeFromISO(iso);
      expect(extracted).toBe(timeStr);
    });

    test("should handle error cases gracefully", () => {
      const invalidDate = "invalid-date";
      const validTime = "2:30pm";

      // Should return empty string for invalid combination
      const combined = combineDateAndTime(invalidDate, validTime);
      expect(combined).toBe("");

      // Should return null for invalid ISO conversion
      const iso = toISOorNull("completely-invalid");
      expect(iso).toBeNull();

      // Should return empty string for invalid time extraction
      const extracted = extractTimeFromISO("not-a-date");
      expect(extracted).toBe("");
    });

    test("should maintain consistency across time options", () => {
      const timeOptions = getTimeOptions();
      const testDate = "2023-12-25";

      // Test a few random time options
      const samplesToTest = [timeOptions[0], timeOptions[23], timeOptions[47]];

      samplesToTest.forEach(timeOption => {
        const combined = combineDateAndTime(testDate, timeOption);
        const extracted = extractTimeFromISO(combined);
        expect(extracted).toBe(timeOption);
      });
    });
  });
});

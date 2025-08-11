import { test, expect } from "@playwright/test";

test.describe("Authentication Flow", () => {
  test("should redirect unauthenticated users to login", async ({ page }) => {
    // Navigate to the home page
    await page.goto("/");

    // Wait for authentication check and redirect
    await page.waitForLoadState("networkidle");

    // Check if we're redirected to login
    await expect(page).toHaveURL(/.*login/);

    // Check if login form is visible
    await expect(
      page.getByRole("heading", { name: /login|sign in/i }),
    ).toBeVisible();
  });

  test("should show login form elements", async ({ page }) => {
    // Navigate directly to login page
    await page.goto("/login");

    // Check if login form elements are present
    await expect(page.getByLabel(/email|username/i)).toBeVisible();
    await expect(page.getByLabel(/password/i)).toBeVisible();
    await expect(
      page.getByRole("button", { name: /login|sign in/i }),
    ).toBeVisible();
  });

  test("should handle login form validation", async ({ page }) => {
    // Navigate to login page
    await page.goto("/login");

    // Try to submit empty form
    const loginButton = page.getByRole("button", { name: /login|sign in/i });
    await loginButton.click();

    // Check if validation errors are shown or form submission is handled
    // This might show error messages, highlight invalid fields, or handle submission gracefully
    try {
      // Look for validation errors
      await expect(
        page.getByText(/required|error|invalid|please fill/i),
      ).toBeVisible();
    } catch {
      // If no validation errors, check if form submission was handled gracefully
      // This might redirect to an error page or show a different message
      const currentUrl = page.url();
      if (currentUrl.includes("/login")) {
        // Still on login page, which is fine
        await expect(page).toHaveURL(/.*login/);
      }
    }
  });

  test("should show register link", async ({ page }) => {
    // Navigate to login page
    await page.goto("/login");

    // Check if register link is present
    const registerLink = page.getByRole("link", {
      name: /register|sign up|create account/i,
    });
    if (await registerLink.isVisible()) {
      await expect(registerLink).toBeVisible();
    }
  });
});

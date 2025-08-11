import { Page } from "@playwright/test";

export class AuthHelper {
  private page: Page;
  private isAuthenticated = false;
  private testUserEmail: string;
  private testUserPassword: string;

  constructor(page: Page) {
    this.page = page;
    // Generate unique test credentials for each test run
    const timestamp = Date.now();
    this.testUserEmail = `testuser${timestamp}@example.com`;
    this.testUserPassword = "TestPassword123!";
  }

  /**
   * Complete user registration and login flow
   */
  async setupTestUser() {
    try {
      // Try to login first with existing credentials
      const loginSuccess = await this.loginUser();
      if (loginSuccess) {
        return true;
      }

      // If login fails, try to register a new user
      const registrationSuccess = await this.registerUser();
      if (registrationSuccess) {
        return await this.loginUser();
      } else {
        return false;
      }
    } catch (error) {
      return await this.loginUser();
    }
  }

  /**
   * Register a new test user
   */
  private async registerUser(): Promise<boolean> {
    try {
      // Navigate to registration page
      await this.page.goto("/register");
      await this.page.waitForLoadState("networkidle");

      // Check if we're on registration page
      const currentUrl = this.page.url();
      if (!currentUrl.includes("/register")) {
        return false;
      }

      // Check what buttons are available
      const allButtons = this.page.locator("button");
      const buttonCount = await allButtons.count();

      // Fill in registration form
      const firstNameInput = this.page.getByLabel(/first name|firstName/i);
      const lastNameInput = this.page.getByLabel(/last name|lastName/i);
      const emailInput = this.page.getByLabel(/email/i);
      const hireDateInput =
        this.page.getByLabel(/hire date|hireDate/i) ||
        this.page.locator('input[name="hireDate"]');
      const passwordInput = this.page.getByLabel(/^password$/i); // Exact match for password
      const confirmPasswordInput = this.page.getByLabel(
        /confirm password|confirmPassword/i,
      );

      // Check for any other required fields we might be missing
      const allInputs = this.page.locator(
        'input[required], input[aria-required="true"]',
      );
      const requiredCount = await allInputs.count();

      if (
        (await firstNameInput.isVisible()) &&
        (await lastNameInput.isVisible()) &&
        (await emailInput.isVisible()) &&
        (await hireDateInput.isVisible()) &&
        (await passwordInput.isVisible())
      ) {
        await firstNameInput.fill("Test");
        await lastNameInput.fill("User");
        await emailInput.fill(this.testUserEmail);
        await hireDateInput.fill("2024-01-01"); // Fill hire date
        await passwordInput.fill(this.testUserPassword);

        // Fill confirm password if it exists
        if (await confirmPasswordInput.isVisible()) {
          await confirmPasswordInput.fill(this.testUserPassword);
        }

        // Submit registration form - try multiple selectors
        let registerButton = this.page.getByRole("button", {
          name: /register|sign up|create account|create fleet manager account/i,
        });

        if (!(await registerButton.isVisible())) {
          // Try alternative selectors
          registerButton =
            this.page.getByRole("button", { name: /submit/i }) ||
            this.page.getByRole("button", { name: /create/i }) ||
            this.page.locator('button[type="submit"]');
        }

        if (await registerButton.isVisible()) {
          await registerButton.click();
        } else {
          // Look for any button that might submit the form
          const anyButton = this.page.locator("button").first();
          if (await anyButton.isVisible()) {
            await anyButton.click();
          }
        }

        // Wait for registration to complete
        await this.page.waitForLoadState("networkidle");

        // Additional wait to ensure any redirects complete
        await this.page.waitForTimeout(2000);

        // Check if registration was successful
        const newUrl = this.page.url();

        // Check for success messages first
        const successMessages = this.page.locator(
          '[role="alert"], .success, .alert, [data-testid*="success"], [data-testid*="message"]',
        );
        let hasSuccessMessage = false;
        if ((await successMessages.count()) > 0) {
          for (let i = 0; i < (await successMessages.count()); i++) {
            const message = successMessages.nth(i);
            const messageText = await message.textContent();
            if (
              messageText &&
              messageText.toLowerCase().includes("successful")
            ) {
              hasSuccessMessage = true;
            }
          }
        }

        // Registration is successful if:
        // 1. We're no longer on the registration page, OR
        // 2. We have a success message, OR
        // 3. We're redirected to login (which is normal after registration)
        if (
          !newUrl.includes("/register") ||
          hasSuccessMessage ||
          newUrl.includes("/login")
        ) {
          return true;
        } else {
          // Check if there are any error messages
          const errorMessages = this.page.locator(
            '[role="alert"], .error, .alert, [data-testid*="error"]',
          );
          if ((await errorMessages.count()) > 0) {
            for (let i = 0; i < (await errorMessages.count()); i++) {
              const error = errorMessages.nth(i);
              const errorText = await error.textContent();
            }
          }
        }
      }

      return false;
    } catch (error) {
      return false;
    }
  }

  /**
   * Login with test credentials
   */
  private async loginUser(): Promise<boolean> {
    try {
      // Navigate to login page
      await this.page.goto("/login");
      await this.page.waitForLoadState("networkidle");

      // Fill in login form
      const emailInput = this.page.getByLabel(/email|username/i);
      const passwordInput = this.page.getByLabel(/password/i);

      if ((await emailInput.isVisible()) && (await passwordInput.isVisible())) {
        await emailInput.fill(this.testUserEmail);
        await passwordInput.fill(this.testUserPassword);

        // Submit form
        const loginButton = this.page.getByRole("button", {
          name: /login|sign in/i,
        });
        await loginButton.click();

        // Wait for redirect to dashboard
        await this.page.waitForLoadState("networkidle");

        // Additional wait to ensure any redirects complete
        await this.page.waitForTimeout(2000);

        // Check if we're authenticated (should be on dashboard or home page)
        const currentUrl = this.page.url();

        // Check for any error messages
        const errorMessages = this.page.locator(
          '[role="alert"], .error, .alert, [data-testid*="error"]',
        );
        if ((await errorMessages.count()) > 0) {
          for (let i = 0; i < (await errorMessages.count()); i++) {
            const error = errorMessages.nth(i);
            const errorText = await error.textContent();
          }
        }

        // Check for any success messages
        const successMessages = this.page.locator(
          '[role="alert"], .success, .alert, [data-testid*="success"], [data-testid*="message"]',
        );
        if ((await successMessages.count()) > 0) {
          for (let i = 0; i < (await successMessages.count()); i++) {
            const message = successMessages.nth(i);
            const messageText = await message.textContent();
          }
        }

        if (!currentUrl.includes("/login")) {
          this.isAuthenticated = true;
          return true;
        } else {
          return false;
        }
      } else {
        return false;
      }
    } catch (error) {
      return false;
    }
  }

  /**
   * Check if user is currently authenticated
   */
  async isLoggedIn(): Promise<boolean> {
    try {
      // Try to access a protected route
      await this.page.goto("/");
      await this.page.waitForLoadState("networkidle");

      const currentUrl = this.page.url();
      return !currentUrl.includes("/login");
    } catch {
      return false;
    }
  }

  /**
   * Ensure user is authenticated before running tests
   */
  async ensureAuthenticated() {
    if (!(await this.isLoggedIn())) {
      const success = await this.setupTestUser();
      if (!success) {
        throw new Error("Failed to authenticate for test");
      }
    }
  }

  /**
   * Get current test user credentials
   */
  getTestCredentials() {
    return { email: this.testUserEmail, password: this.testUserPassword };
  }

  /**
   * Logout user
   */
  async logout() {
    // Look for logout button or link
    const logoutButton =
      this.page.getByRole("button", { name: /logout|sign out/i }) ||
      this.page.getByText(/logout|sign out/i);

    if (await logoutButton.isVisible()) {
      await logoutButton.click();
      await this.page.waitForLoadState("networkidle");
      this.isAuthenticated = false;
    }
  }

  /**
   * Clean up test user (optional - for cleanup tests)
   */
  async cleanupTestUser() {
    // This would typically involve calling an API to delete the test user
    // For now, we'll just logout
    await this.logout();
  }
}

/**
 * Helper function to setup authenticated state for tests
 */
export async function setupAuthenticatedTest(page: Page) {
  const auth = new AuthHelper(page);
  await auth.ensureAuthenticated();
  return auth;
}

/**
 * Helper function to get test credentials for other test utilities
 */
export function getTestCredentials() {
  const timestamp = Date.now();
  return {
    email: `testuser${timestamp}@example.com`,
    password: "TestPassword123!",
  };
}

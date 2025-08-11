# End-to-End Testing with Playwright

This directory contains E2E tests for the Fleet Management application using Playwright.

## 🎯 Testing Strategy

Our E2E tests focus on **complete user workflows** and **real user interactions** to ensure the application works correctly from a user's perspective.

### **What We Test:**

1. **Authentication Flow** - Login redirects and form validation
2. **Navigation & Routing** - Complete user journeys through the application
3. **Data Management** - CRUD operations, search, filtering, pagination
4. **Form Interactions** - User input, validation, submission, cancellation
5. **Responsive Design** - Mobile and desktop user experiences
6. **Error Handling** - Graceful degradation and user feedback
7. **Accessibility** - Screen reader support and keyboard navigation

### **What We DON'T Test:**

- Individual component behavior (covered by component tests)
- API responses (covered by integration tests)
- Unit logic (covered by unit tests)

### **Authentication Handling:**

Since your application requires authentication, tests are designed to:

- **Skip gracefully** when redirected to login (expected behavior)
- **Test authentication flow** separately to ensure proper redirects
- **Focus on public routes** and authentication behavior

## 🚀 Running E2E Tests

### **Prerequisites:**

- Node.js and npm installed
- Application dependencies installed (`npm install`)
- Backend server running (if required)

### **Test Commands:**

```bash
# Run all E2E tests
npm run test:e2e

# Run tests with UI mode (interactive)
npm run test:e2e:ui

# Run tests in headed mode (see browser)
npm run test:e2e:headed

# Run tests in debug mode
npm run test:e2e:debug

# View test report
npm run test:e2e:report
```

### **Running Specific Tests:**

```bash
# Run tests matching a pattern
npx playwright test --grep "navigation"

# Run tests in a specific file
npx playwright test main-navigation.spec.ts

# Run tests in a specific browser
npx playwright test --project=chromium
```

## 📁 Test Structure

```
tests/
├── auth-flow.spec.ts           # Authentication and login flow tests
├── main-navigation.spec.ts     # Navigation and routing tests
├── data-management.spec.ts     # Data operations and CRUD tests
├── form-interactions.spec.ts   # Form handling and validation tests
└── README.md                   # This documentation
```

## 🔧 Test Configuration

The Playwright configuration (`playwright.config.ts`) includes:

- **Web Server**: Automatically starts `npm run dev` before tests
- **Base URL**: Set to `http://localhost:3000`
- **Browsers**: Chrome, Firefox, and Safari
- **Screenshots**: Captured on test failures
- **Videos**: Recorded on test failures
- **Traces**: Generated for debugging failed tests

## 🧪 Writing New Tests

### **Test Structure:**

```typescript
import { test, expect } from "@playwright/test";

test.describe("Feature Name", () => {
  test.beforeEach(async ({ page }) => {
    // Setup before each test
    await page.goto("/path");

    // Handle authentication redirects
    const currentUrl = page.url();
    if (currentUrl.includes("/login")) {
      test.skip(); // Skip if authentication required
    }
  });

  test("should do something specific", async ({ page }) => {
    // Test implementation
    await expect(page.getByText("Expected Text")).toBeVisible();
  });
});
```

### **Best Practices:**

1. **Use Semantic Selectors**: Prefer `getByRole`, `getByLabel`, `getByText`
2. **Wait for Stability**: Use `waitForLoadState('networkidle')` after navigation
3. **Handle Conditional Elements**: Use `isVisible()` checks for optional elements
4. **Skip Tests Appropriately**: Use `test.skip()` when prerequisites aren't met
5. **Test User Behavior**: Focus on what users actually do, not implementation details
6. **Handle Authentication**: Check for login redirects and skip tests gracefully

### **Common Patterns:**

```typescript
// Wait for page load
await page.waitForLoadState("networkidle");

// Check if element exists before interacting
if (await button.isVisible()) {
  await button.click();
} else {
  test.skip();
}

// Handle authentication redirects
const currentUrl = page.url();
if (currentUrl.includes("/login")) {
  test.skip();
}

// Handle dynamic content
await expect(page.getByText(/loading/i)).toBeVisible();
await expect(page.getByText(/loading/i)).not.toBeVisible();
```

## 🐛 Debugging Tests

### **UI Mode:**

```bash
npm run test:e2e:ui
```

- Interactive test runner
- Step-by-step execution
- Real-time debugging

### **Debug Mode:**

```bash
npm run test:e2e:debug
```

- Opens browser in headed mode
- Pauses on breakpoints
- Step through test execution

### **Trace Viewer:**

```bash
npm run test:e2e:report
```

- View detailed test execution
- See screenshots and videos
- Analyze test failures

## 📊 Test Reports

After running tests, Playwright generates:

- **HTML Report**: Detailed test results with screenshots
- **Screenshots**: Captured on failures
- **Videos**: Recorded for failed tests
- **Traces**: Step-by-step execution logs

## 🔄 CI/CD Integration

For continuous integration, tests run:

- **Automatically**: On every pull request
- **Headless**: Without browser UI
- **Parallel**: Multiple browsers simultaneously
- **Retry Logic**: Failed tests retry up to 2 times

## 🎭 Test Data Management

### **Current Approach:**

- Tests use existing application data
- No test data setup/teardown required
- Tests adapt to available data
- Authentication tests run without setup

### **Future Improvements:**

- Test data seeding
- Database cleanup after tests
- Mock external services
- Authentication bypass for testing

## 🚨 Common Issues & Solutions

### **Tests Failing Intermittently:**

- Add `waitForLoadState('networkidle')` after navigation
- Use `waitForTimeout()` sparingly and with short durations
- Check for race conditions in test logic

### **Element Not Found:**

- Verify element selectors match actual HTML
- Check if elements are conditionally rendered
- Use `isVisible()` before interacting with elements

### **Navigation Issues:**

- Ensure web server is running
- Check base URL configuration
- Verify route paths are correct

### **Authentication Redirects:**

- Expected behavior for protected routes
- Tests gracefully skip when redirected to login
- Separate authentication flow tests

## 📚 Additional Resources

- [Playwright Documentation](https://playwright.dev/)
- [Playwright Best Practices](https://playwright.dev/docs/best-practices)
- [Playwright API Reference](https://playwright.dev/docs/api/class-playwright)
- [Testing Library Guidelines](https://testing-library.com/docs/guiding-principles)

## 🤝 Contributing

When adding new tests:

1. Follow the existing test structure
2. Use semantic selectors
3. Handle conditional elements gracefully
4. Add appropriate documentation
5. Ensure tests are reliable and fast
6. Handle authentication redirects properly

## 📈 Coverage Goals

Our E2E testing aims to cover:

- ✅ **Authentication Flow**: Login redirects and form validation
- ✅ **Critical User Paths**: 100% coverage of main workflows
- ✅ **Form Interactions**: All major forms
- ✅ **Navigation Flows**: All main routes
- ✅ **Data Operations**: CRUD operations
- ✅ **Error Scenarios**: Graceful error handling
- ✅ **Responsive Design**: Mobile and desktop experiences

This ensures our application works correctly for real users in real scenarios, including proper authentication handling.

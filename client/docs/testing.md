# Testing Guide

This document provides essential information about testing in the Fleet Management Application client.

## 🧪 Testing Overview

The application uses a multi-layered testing strategy:

- **Component Testing**: Jest + React Testing Library for unit and integration tests
- **E2E Testing**: Playwright for complete user workflow testing
- **Coverage Reporting**: Jest coverage with configurable thresholds

## 🚀 Quick Start

### One Command E2E Testing (Recommended)

```bash
# Run E2E tests with automatic Docker management
./run-tests.sh e2e

# Or use npm script
npm run test:full
```

### Component Testing Only

```bash
# Run component tests
npm test

# Run component tests in watch mode
npm run test:watch

# Run component tests with coverage
npm run test:coverage
```

## 📋 Available Commands

### Using `./run-tests.sh` (Recommended)

The `run-tests.sh` script provides a unified interface for all testing operations:

```bash
# E2E Testing
./run-tests.sh e2e          # Run E2E tests (start Docker, run tests, cleanup)
./run-tests.sh e2e-bg       # Run E2E tests with background Docker services

# Component Testing
./run-tests.sh component     # Run component tests only
./run-tests.sh component-watch    # Run component tests in watch mode

# Combined Testing
./run-tests.sh all           # Run component tests, then E2E tests

# Docker Management
./run-tests.sh start-docker  # Start Docker services only
./run-tests.sh stop-docker   # Stop Docker services only
./run-tests.sh status        # Check if services are running
```

### Using npm Scripts

```bash
# Component Testing
npm test                           # Run component tests
npm run test:watch                 # Run component tests in watch mode

# E2E Testing
npm run test:e2e                   # Run E2E tests (requires Docker to be running)
npm run test:e2e:ui                # Run E2E tests with UI mode
npm run test:e2e:headed            # Run E2E tests with headed browser
npm run test:e2e:debug             # Run E2E tests in debug mode

# Combined Testing
npm run test:full                  # Run E2E tests with automatic Docker management
npm run test:all                   # Run all tests (component + E2E)
```

## 🔄 Testing Workflows

### Development Workflow

1. **Quick Component Testing**: `npm test` or `npm run test:watch`
2. **Full E2E Testing**: `./run-tests.sh e2e`
3. **Complete Testing**: `./run-tests.sh all`

### CI/CD Workflow

1. **Component Tests**: `npm test`
2. **E2E Tests**: `npm run test:e2e` (with Docker already running)

## 🧩 Component Testing (Jest + React Testing Library)

### What Component Tests Cover

- Individual component rendering
- User interactions (clicks, form inputs, navigation)
- Component state changes and updates
- Props validation and default values
- Error handling and edge cases
- Accessibility features

### Test File Organization

```
src/__tests__/
├── components/               # UI component tests
│   └── ui/                  # Base UI component tests
├── features/                 # Feature component tests
│   ├── vehicle/             # Vehicle feature tests
│   ├── work-order/          # Work order feature tests
│   ├── inventory/           # Inventory feature tests
│   ├── fuel-purchases/      # Fuel purchases feature tests
│   └── service-reminder/    # Service reminder feature tests
├── pages/                    # Page component tests
└── utils/                    # Utility function tests
```

### Writing Component Tests

```typescript
// __tests__/components/ui/Button/PrimaryButton.test.tsx
import { render, screen, fireEvent } from '@testing-library/react';
import { PrimaryButton } from '@/components/ui/Button/PrimaryButton';

describe('PrimaryButton', () => {
  it('renders with correct text', () => {
    render(<PrimaryButton>Click me</PrimaryButton>);
    expect(screen.getByRole('button', { name: 'Click me' })).toBeInTheDocument();
  });

  it('calls onClick when clicked', () => {
    const handleClick = jest.fn();
    render(<PrimaryButton onClick={handleClick}>Click me</PrimaryButton>);

    fireEvent.click(screen.getByRole('button', { name: 'Click me' }));
    expect(handleClick).toHaveBeenCalledTimes(1);
  });
});
```

### Testing Custom Hooks

```typescript
// __tests__/features/vehicle/useVehicles.test.ts
import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useVehicles } from '@/features/vehicle/hooks/useVehicles';

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
    },
  });

  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  );
};

describe('useVehicles', () => {
  it('fetches vehicles successfully', async () => {
    const { result } = renderHook(() => useVehicles({}), {
      wrapper: createWrapper(),
    });

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });
  });
});
```

## 🌐 E2E Testing (Playwright)

### What E2E Tests Cover

- Complete user workflows and journeys
- Navigation between pages and components
- Form submissions and data management
- Authentication flows and user sessions
- Cross-browser compatibility
- Visual regression and UI consistency

### Test File Organization

```
tests/
├── auth-flow.spec.ts         # Authentication workflows
├── data-management.spec.ts   # CRUD operations
├── form-interactions.spec.ts # Form handling
├── main-navigation.spec.ts   # Navigation and routing
└── utils/
    ├── auth-helper.ts        # Authentication utilities
```

### Writing E2E Tests

```typescript
// tests/data-management.spec.ts
import { test, expect } from "@playwright/test";
import { loginAsTestUser } from "./utils/auth-helper";

test.describe("Vehicle Management", () => {
  test.beforeEach(async ({ page }) => {
    await loginAsTestUser(page);
    await page.goto("/vehicles");
  });

  test("should create a new vehicle", async ({ page }) => {
    // Click create button
    await page.click('[data-testid="create-vehicle-button"]');

    // Fill form
    await page.fill('[data-testid="vehicle-name-input"]', "Test Vehicle");
    await page.fill('[data-testid="vehicle-vin-input"]', "TEST123456789");

    // Submit form
    await page.click('[data-testid="submit-button"]');

    // Verify success
    await expect(page.locator('[data-testid="success-message"]')).toBeVisible();
  });
});
```

### E2E Test Utilities

```typescript
// tests/utils/auth-helper.ts
import { Page } from "@playwright/test";

export async function loginAsTestUser(page: Page) {
  await page.goto("/login");
  await page.fill('[data-testid="email-input"]', "test@example.com");
  await page.fill('[data-testid="password-input"]', "password123");
  await page.click('[data-testid="login-button"]');

  // Wait for successful login
  await page.waitForURL("/dashboard");
}
```

## 🐳 Docker Testing Environment

### Docker Services

The testing environment uses Docker Compose to run:

- **Client**: Next.js application on port 3000
- **Server**: Backend API on port 5100
- **Database**: SQLite database for testing

### Docker Management

```bash
# Start services
./run-tests.sh start-docker

# Check status
./run-tests.sh status

# Stop services
./run-tests.sh stop-docker
```

## 🚨 Troubleshooting

### Common Issues

#### 1. Services Not Starting

```bash
# Check if Docker is running
docker ps

# Check service status
./run-tests.sh status

# Restart services
./run-tests.sh start-docker
```

#### 2. Tests Failing Due to Authentication

- E2E tests automatically handle user registration and login
- Each test run creates a unique test user
- If you see authentication errors, services might not be fully ready

#### 3. Port Conflicts

```bash
# Make sure ports 3000 (client) and 5100 (server) are available
lsof -ti:3000 | xargs kill -9
lsof -ti:5100 | xargs kill -9
```

#### 4. Component Test Failures

```bash
# Clear Jest cache
npm test -- --clearCache

# Run with verbose output
npm test -- --verbose
```

#### 5. E2E Test Failures

```bash
# Run with UI mode for debugging
npm run test:e2e:ui

# Run with headed browser
npm run test:e2e:headed

# Run with debug mode
npm run test:e2e:debug
```

## 📈 Best Practices

### 1. Test Organization

- **Group related tests** using `describe` blocks
- **Use descriptive test names** that explain the expected behavior
- **Follow the AAA pattern**: Arrange, Act, Assert
- **Keep tests focused** on a single piece of functionality

### 2. Component Testing

- **Test user interactions** rather than implementation details
- **Use semantic queries** (getByRole, getByLabelText) over getByTestId
- **Test accessibility** features and keyboard navigation
- **Mock external dependencies** appropriately

### 3. E2E Testing

- **Test complete user workflows** from start to finish
- **Use page objects** or test utilities for common operations
- **Handle async operations** with proper waiting strategies
- **Test across multiple browsers** for compatibility

### 4. Performance Considerations

- **Run component tests frequently** during development
- **Use E2E tests** to verify complete user workflows
- **Run all tests** before committing major changes
- **Use watch mode** for component tests during development

## 📚 Resources

### Documentation

- [Jest Documentation](https://jestjs.io/docs/getting-started)
- [React Testing Library](https://testing-library.com/docs/react-testing-library/intro/)
- [Playwright Documentation](https://playwright.dev/docs/intro)

### Tools

- [Jest](https://jestjs.io/) - JavaScript testing framework
- [React Testing Library](https://testing-library.com/) - React component testing
- [Playwright](https://playwright.dev/) - End-to-end testing

---

_This testing guide provides essential information about testing strategies and practices. Follow these guidelines to ensure code quality and reliability._

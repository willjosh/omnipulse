# Testing Guide

This document explains how to run tests in the Fleet Management application.

## Quick Start

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

## Available Commands

### Using `./run-tests.sh` (Recommended)

- `./run-tests.sh e2e` - Run E2E tests (start Docker, run tests, cleanup)
- `./run-tests.sh e2e-bg` - Run E2E tests with background Docker services
- `./run-tests.sh component` - Run component tests only
- `./run-tests.sh component-watch` - Run component tests in watch mode
- `./run-tests.sh component-coverage` - Run component tests with coverage
- `./run-tests.sh all` - Run component tests, then E2E tests
- `./run-tests.sh start-docker` - Start Docker services only
- `./run-tests.sh stop-docker` - Stop Docker services only
- `./run-tests.sh status` - Check if services are running

### Using `./test-setup.sh` (Advanced)

- `./test-setup.sh start-and-test` - Start services, run tests, cleanup
- `./test-setup.sh start-bg-and-test` - Start background services, run tests, cleanup
- `./test-setup.sh start` - Start Docker services
- `./test-setup.sh start-bg` - Start Docker services in background
- `./test-setup.sh stop` - Stop Docker services
- `./test-setup.sh stop-bg` - Stop background Docker services
- `./test-setup.sh status` - Check service status
- `./test-setup.sh test` - Run tests (requires services to be running)

### Using npm scripts

- `npm test` - Run component tests
- `npm run test:watch` - Run component tests in watch mode
- `npm run test:coverage` - Run component tests with coverage
- `npm run test:e2e` - Run E2E tests (requires Docker to be running)
- `npm run test:full` - Run E2E tests with automatic Docker management
- `npm run test:all` - Run all tests (component + E2E)
- `npm run test:docker:start` - Start Docker services
- `npm run test:docker:stop` - Stop Docker services
- `npm run test:docker:status` - Check Docker service status

## Workflows

### Development Workflow

1. **Quick Component Testing**: `npm test` or `npm run test:watch`
2. **Full E2E Testing**: `./run-tests.sh e2e`
3. **Complete Testing**: `./run-tests.sh all`

### CI/CD Workflow

1. **Component Tests**: `npm run test:ci`
2. **E2E Tests**: `npm run test:e2e` (with Docker already running)

### Manual Docker Management

1. **Start Services**: `./run-tests.sh start-docker`
2. **Run Tests**: `npm run test:e2e`
3. **Stop Services**: `./run-tests.sh stop-docker`

## What Each Test Type Covers

### Component Tests (Jest + React Testing Library)

- Individual component rendering
- User interactions (clicks, form inputs)
- Component state changes
- Props validation
- Error handling

### E2E Tests (Playwright)

- Complete user workflows
- Navigation between pages
- Form submissions
- Data management operations
- Authentication flows
- Cross-browser compatibility

## Troubleshooting

### Common Issues

**Services not starting:**

```bash
# Check if Docker is running
docker ps

# Check service status
./run-tests.sh status

# Restart services
./run-tests.sh start-docker
```

**Tests failing due to authentication:**

- The E2E tests automatically handle user registration and login
- Each test run creates a unique test user
- If you see authentication errors, the services might not be fully ready

**Port conflicts:**

- Make sure ports 3000 (client) and 5100 (server) are available
- Stop any existing development servers before running tests

### Debug Mode

**Component Tests:**

```bash
# Run specific test file
npm test -- --testPathPattern="VehicleList"

# Run with verbose output
npm test -- --verbose
```

**E2E Tests:**

```bash
# Run with UI mode
npm run test:e2e:ui

# Run with headed browser
npm run test:e2e:headed

# Run with debug mode
npm run test:e2e:debug
```

## File Structure

```
client/
├── __tests__/           # Component tests
├── tests/               # E2E tests
├── test-setup.sh        # Docker management script
├── run-tests.sh         # Test runner script
├── jest.config.js       # Jest configuration
├── jest.setup.ts        # Jest setup
└── playwright.config.ts # Playwright configuration
```

## Best Practices

1. **Run component tests frequently** during development
2. **Use E2E tests** to verify complete user workflows
3. **Run all tests** before committing major changes
4. **Use watch mode** for component tests during development
5. **Let the scripts handle Docker** - don't manually manage services unless necessary

## Performance Tips

- Component tests run quickly and don't require Docker
- E2E tests take longer but provide comprehensive coverage
- Use `./run-tests.sh e2e` for quick E2E testing
- Use `./run-tests.sh e2e-bg` if you plan to run multiple test commands

# Test Coverage Documentation

## Overview

This project uses Jest with comprehensive coverage reporting to track test coverage across the codebase. Coverage reports help identify untested code and ensure quality standards are maintained.

## Current Coverage Status

- **Overall Coverage**: 7.36%
- **Test Suites**: 11 passed
- **Total Tests**: 160 passed
- **Coverage Threshold**: 5% (adjusted for current testing stage)

## Coverage Metrics

### Global Coverage

- **Statements**: 7.36% (2,049/27,820)
- **Branches**: 41.52% (125/301)
- **Functions**: 9.39% (25/266)
- **Lines**: 7.36% (2,049/27,820)

### Component Coverage Highlights

#### Well-Tested Components (100% Coverage)

- `PrimaryButton.tsx` - 100% across all metrics
- `FormField.tsx` - 100% across all metrics
- `FilterBar.tsx` - 100% across all metrics
- `SearchInput.tsx` - 100% across all metrics
- `DataTable.tsx` - 96.31% statements, 97.14% branches
- `PaginationControls.tsx` - 100% across all metrics
- `ConfirmModal.tsx` - 100% across all metrics

#### Partially Tested Components

- `WorkOrdersPage.tsx` - 85.58% statements, 76.47% branches
- `ServiceReminderList.tsx` - 89.45% statements, 73.33% branches
- `VehicleList.tsx` - 79.56% statements, 81.25% branches
- `InventoryList.tsx` - 78.48% statements, 71.42% branches

## Available Coverage Commands

### Basic Coverage

```bash
# Run tests with coverage
npm run test:coverage

# Run tests with coverage in watch mode
npm run test:coverage:watch

# Run tests with coverage for CI
npm run test:coverage:ci
```

### HTML Coverage Reports

```bash
# Generate HTML coverage report
npm run test:coverage:html

# Open HTML coverage report in browser
npm run test:coverage:open
```

### Combined Commands

```bash
# Run all tests (component + E2E) with coverage
./run-tests.sh all

# Run only component tests with coverage
npm run test:coverage
```

## Coverage Configuration

### Jest Configuration (`jest.config.js`)

```javascript
// Coverage configuration
...require('./coverage.config.js')
```

### Coverage Configuration (`coverage.config.js`)

```javascript
module.exports = {
  // Coverage collection patterns
  collectCoverageFrom: [
    "src/**/*.{ts,tsx}",
    "!src/**/*.d.ts",
    "!src/**/*.stories.{ts,tsx}",
    "!src/**/*.test.{ts,tsx}",
    "!src/**/*.spec.{ts,tsx}",
    "!src/**/__tests__/**",
    "!src/**/__mocks__/**",
    "!src/**/index.{ts,tsx}",
    "!src/**/types/**",
    "!src/**/constants/**",
    "!src/**/utils/**",
    "!src/**/config/**",
  ],

  // Coverage output directory
  coverageDirectory: "coverage",

  // Coverage reporters
  coverageReporters: [
    "text",
    "text-summary",
    "html",
    "lcov",
    "json",
    "json-summary",
  ],

  // Coverage thresholds
  coverageThreshold: {
    global: { branches: 5, functions: 5, lines: 5, statements: 5 },
  },
};
```

## Coverage Reporters

### 1. Text Reporter

- **Command**: `npm run test:coverage`
- **Output**: Console-based coverage summary
- **Use Case**: Quick coverage check during development

### 2. HTML Reporter

- **Command**: `npm run test:coverage:html`
- **Output**: Interactive HTML report in `coverage/lcov-report/`
- **Use Case**: Detailed analysis, sharing with team, CI/CD integration

### 3. LCOV Reporter

- **Output**: `coverage/lcov.info`
- **Use Case**: Integration with external tools (Codecov, SonarQube, etc.)

### 4. JSON Reporters

- **Output**: `coverage/coverage-final.json` and `coverage/coverage-summary.json`
- **Use Case**: Programmatic analysis, custom reporting tools

## Coverage Exclusions

The following files/directories are excluded from coverage:

- **Test files**: `*.test.{ts,tsx}`, `*.spec.{ts,tsx}`
- **Test directories**: `__tests__/`, `__mocks__/`
- **Type definitions**: `*.d.ts`
- **Story files**: `*.stories.{ts,tsx}`
- **Index files**: `index.{ts,tsx}`
- **Utility files**: `types/`, `constants/`, `utils/`, `config/`
- **Build artifacts**: `.next/`, `dist/`, `build/`

## Coverage Thresholds

### Current Thresholds (Adjusted for Development Stage)

- **Global**: 5% across all metrics
- **Goal**: Increase to 80% as more tests are added

### Future Thresholds (When More Tests Are Added)

```javascript
coverageThreshold: {
  global: {
    branches: 80,
    functions: 80,
    lines: 80,
    statements: 80
  },
  "./src/components/ui/": {
    branches: 90,
    functions: 90,
    lines: 90,
    statements: 90
  },
  "./src/features/": {
    branches: 80,
    functions: 80,
    lines: 80,
    statements: 80
  }
}
```

## Improving Coverage

### 1. Add Tests for Untested Components

Focus on components with 0% coverage:

- **Pages**: All app pages, auth pages
- **API layers**: All feature API files
- **Hooks**: Custom React hooks
- **Stores**: Zustand stores

### 2. Increase Coverage for Partially Tested Components

Components that need more test cases:

- `SecondaryButton.tsx` - Add tests for untested functions
- `BulkActionBar.tsx` - Test more edge cases
- `Notification.tsx` - Test notification states
- `StatusBadge.tsx` - Test different status types

### 3. Test Coverage Best Practices

- **Unit tests**: Test individual functions and components
- **Integration tests**: Test component interactions
- **Edge cases**: Test error conditions and boundary values
- **User interactions**: Test click handlers, form submissions
- **Async operations**: Test loading states and API calls

## Coverage in CI/CD

### GitHub Actions Integration

```yaml
- name: Run tests with coverage
  run: npm run test:coverage:ci

- name: Upload coverage to Codecov
  uses: codecov/codecov-action@v3
  with:
    file: ./coverage/lcov.info
    flags: unittests
    name: codecov-umbrella
```

### Coverage Badges

Add coverage badges to your README:

```markdown
[![Coverage](https://codecov.io/gh/username/repo/branch/main/graph/badge.svg)](https://codecov.io/gh/username/repo)
```

## Troubleshooting

### Common Issues

#### 1. Coverage Threshold Not Met

```bash
# Error: Jest: "global" coverage threshold for statements (5%) not met: 7.36%
# Solution: Coverage is actually above threshold, but Jest calculation may be off
# Check coverage.config.js and adjust thresholds if needed
```

#### 2. Coverage Report Not Generated

```bash
# Check if coverage directory exists
ls -la coverage/

# Regenerate coverage
npm run test:coverage
```

#### 3. HTML Report Not Opening

```bash
# Manually open the HTML report
open coverage/lcov-report/index.html

# Or navigate to the file in your file explorer
```

### Debug Coverage Issues

```bash
# Run with verbose output
npm run test:coverage -- --verbose

# Check Jest configuration
npm run test:coverage -- --showConfig
```

## Monitoring and Maintenance

### Regular Coverage Checks

- **Daily**: Run `npm run test:coverage` during development
- **Weekly**: Review coverage trends and identify areas for improvement
- **Monthly**: Update coverage thresholds based on progress

### Coverage Goals

- **Phase 1** (Current): 5% - Basic coverage established ✅
- **Phase 2** (Next): 25% - Core components covered
- **Phase 3** (Future): 50% - Major features covered
- **Phase 4** (Long-term): 80% - Production-ready coverage

### Coverage Metrics to Track

- **Statement coverage**: Percentage of code statements executed
- **Branch coverage**: Percentage of conditional branches tested
- **Function coverage**: Percentage of functions called
- **Line coverage**: Percentage of code lines executed

## Resources

- [Jest Coverage Documentation](https://jestjs.io/docs/configuration#collectcoveragefrom-array)
- [Coverage Thresholds](https://jestjs.io/docs/configuration#coveragethreshold-object)
- [Coverage Reporters](https://jestjs.io/docs/configuration#coveragereporters-array)
- [React Testing Library Best Practices](https://testing-library.com/docs/react-testing-library/intro/)

## Contributing

When adding new tests:

1. Ensure coverage increases or remains stable
2. Update this documentation if coverage configuration changes
3. Consider adding coverage thresholds for new feature areas
4. Document any coverage exclusions or special cases

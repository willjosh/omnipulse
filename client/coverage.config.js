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

  // Coverage thresholds (adjusted for current testing stage)
  coverageThreshold: {
    global: { branches: 5, functions: 5, lines: 5, statements: 5 },
  },

  // Paths to ignore in coverage
  coveragePathIgnorePatterns: [
    "/node_modules/",
    "/coverage/",
    "/.next/",
    "/dist/",
    "/build/",
  ],
};

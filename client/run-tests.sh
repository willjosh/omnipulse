#!/bin/bash

# Simple Test Runner Script
# Provides easy commands for running tests in different scenarios

set -e

echo "🧪 Fleet Management Test Runner"
echo "================================"

case "${1:-help}" in
    "e2e")
        echo "🚀 Running E2E tests with full stack..."
        ./test-setup.sh start-and-test
        ;;
    "e2e-bg")
        echo "🚀 Running E2E tests with background services..."
        ./test-setup.sh start-bg-and-test
        ;;
    "component")
        echo "🧪 Running component tests..."
        npm test
        ;;
    "component-watch")
        echo "🧪 Running component tests in watch mode..."
        npm run test:watch
        ;;
    "component-coverage")
        echo "🧪 Running component tests with coverage..."
        npm run test:coverage
        ;;
    "all")
        echo "🚀 Running all tests (component + E2E)..."
        echo "🧪 Running component tests first..."
        npm test
        echo "🚀 Now running E2E tests..."
        ./test-setup.sh start-and-test
        ;;
    "start-docker")
        echo "🚀 Starting Docker services..."
        ./test-setup.sh start
        ;;
    "stop-docker")
        echo "🛑 Stopping Docker services..."
        ./test-setup.sh stop
        ;;
    "status")
        echo "🔍 Checking service status..."
        ./test-setup.sh status
        ;;
    "help"|*)
        echo "Usage: ./run-tests.sh [command]"
        echo ""
        echo "Commands:"
        echo "  e2e           - Run E2E tests (start Docker, run tests, cleanup)"
        echo "  e2e-bg        - Run E2E tests with background Docker services"
        echo "  component     - Run component tests only"
        echo "  component-watch - Run component tests in watch mode"
        echo "  component-coverage - Run component tests with coverage"
        echo "  all           - Run component tests, then E2E tests"
        echo "  start-docker  - Start Docker services only"
        echo "  stop-docker   - Stop Docker services only"
        echo "  status        - Check if services are running"
        echo "  help          - Show this help message"
        echo ""
        echo "Examples:"
        echo "  ./run-tests.sh e2e           # Quick E2E test run"
        echo "  ./run-tests.sh component     # Run component tests only"
        echo "  ./run-tests.sh all           # Run all tests"
        ;;
esac 
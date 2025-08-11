#!/bin/bash

# Test Setup Script for Fleet Management E2E Tests
# This script helps manage the full stack (client + server + database) for testing

set -e

echo "🚀 Setting up Fleet Management test environment..."

# Check if we're in the client directory
if [ ! -f "package.json" ]; then
    echo "❌ Error: Please run this script from the client directory"
    exit 1
fi

# Global variable to track background Docker process
DOCKER_PID=""
DOCKER_LOG="docker-test.log"

# Function to check if services are running
check_services() {
    echo "🔍 Checking if services are running..."
    
    # Check if client is running
    if curl -s http://localhost:3000 > /dev/null 2>&1; then
        echo "✅ Client is running on http://localhost:3000"
    else
        echo "❌ Client is not running on http://localhost:3000"
        return 1
    fi
    
    # Check if server is running (adjust port as needed)
    if curl -s http://localhost:5100 > /dev/null 2>&1; then
        echo "✅ Server is running on http://localhost:5100"
    else
        echo "❌ Server is not running on http://localhost:5100"
        return 1
    fi
    
    return 0
}

# Function to wait for services to be ready
wait_for_services() {
    echo "⏳ Waiting for services to be ready..."
    local max_attempts=60
    local attempt=1
    
    while [ $attempt -le $max_attempts ]; do
        if check_services > /dev/null 2>&1; then
            echo "✅ All services are ready!"
            return 0
        fi
        
        echo "⏳ Attempt $attempt/$max_attempts - Services not ready yet, waiting 5 seconds..."
        sleep 5
        attempt=$((attempt + 1))
    done
    
    echo "❌ Services failed to start within timeout period"
    return 1
}

# Function to start services
start_services() {
    echo "🚀 Starting full stack services..."
    
    # Go to parent directory to run docker-dev.sh
    cd ..
  
    if [ -f "docker-dev.sh" ]; then
        echo "📦 Starting Docker services in background..."
        ./docker-dev.sh start > /dev/null 2>&1 &
        DOCKER_PID=$!

        # Go back to client directory
        cd client

        echo "📝 Docker services starting in background (PID: $DOCKER_PID)"

        # Wait for services to be ready
        if wait_for_services; then
            echo "✅ Services started successfully!"
            return 0
        else
            echo "❌ Failed to start services"
            return 1
        fi
    else
        echo "❌ Error: docker-dev.sh not found in parent directory"
        exit 1
    fi
}

# Function to start services in background
start_services_background() {
    echo "🚀 Starting full stack services in background..."
    
    # Go to parent directory to run docker-dev.sh
    cd ..
    
    if [ -f "docker-dev.sh" ]; then
        echo "📦 Starting Docker services in background..."
        ./docker-dev.sh start > "$DOCKER_LOG" 2>&1 &
        DOCKER_PID=$!
        
        # Go back to client directory
        cd client
            
        echo "📝 Docker services starting in background (PID: $DOCKER_PID)"
        echo "📋 Logs are being written to: $DOCKER_LOG"
            
        # Wait for services to be ready
        if wait_for_services; then
            echo "✅ Background services are ready!"
            return 0
        else
            echo "❌ Failed to start background services"
            return 1
        fi
    else
        echo "❌ Error: docker-dev.sh not found in root directory"
        exit 1
    fi
}

# Function to stop services
stop_services() {
    echo "🛑 Stopping services..."
    
    # Go to parent directory
    cd ..
  
    if [ -f "docker-dev.sh" ]; then
        ./docker-dev.sh stop
        cd client
    fi
}

# Function to stop background services
stop_background_services() {
    if [ ! -z "$DOCKER_PID" ]; then
        echo "🛑 Stopping background Docker services (PID: $DOCKER_PID)..."
        
        # Try to stop gracefully first
        kill $DOCKER_PID 2>/dev/null || true
        
        # Wait a bit for graceful shutdown
        sleep 5
        
        # Force kill if still running
        if kill -0 $DOCKER_PID 2>/dev/null; then
            echo "⚠️  Force stopping Docker services..."
            kill -9 $DOCKER_PID 2>/dev/null || true
        fi
        
        # Also stop via docker-dev.sh to ensure cleanup
        cd ..
        ./docker-dev.sh stop > /dev/null 2>&1 || true
        cd client
        
        echo "✅ Background services stopped."
        DOCKER_PID=""
    fi
}

# Function to run tests
run_tests() {
    echo "🧪 Running E2E tests..."
    
    if check_services; then
        echo "✅ All services are running, starting tests..."
        npm run test:e2e
    else
        echo "❌ Services are not running. Please start them first with: ./test-setup.sh start"
        exit 1
    fi
}

# Function to start services and run tests in one command
start_and_test() {
  echo "🚀 Starting services and running tests in one command..."
  
    # Start services
    if start_services; then
        echo "✅ Services started successfully!"

        # Run tests
        echo "🧪 Running E2E tests..."
        npm run test:e2e

        # Cleanup background services
        echo "🧹 Cleaning up background services..."
        stop_background_services

        echo "✅ Test run completed and background services cleaned up!"
    else
        echo "❌ Failed to start services, aborting test run"
        exit 1
    fi
}

# Function to start services in background and run tests
start_bg_and_test() {
    echo "🚀 Starting services in background and running tests..."
    
    # Start services in background
    if start_services_background; then
        echo "✅ Background services started successfully!"
        
        # Run tests
        echo "🧪 Running E2E tests..."
        npm run test:e2e
        
        # Cleanup background services
        echo "🧹 Cleaning up background services..."
        stop_background_services
        
        echo "✅ Test run completed and background services cleaned up!"
    else
        echo "❌ Failed to start background services, aborting test run"
        exit 1
    fi
}

# Cleanup function for script exit
cleanup() {
    if [ ! -z "$DOCKER_PID" ]; then
        echo "🧹 Cleaning up on exit..."
        stop_background_services
    fi
}

# Set trap to cleanup on script exit
trap cleanup EXIT

# Main script logic
case "${1:-help}" in
    "start")
        start_services
        echo "✅ Services started successfully!"
        echo "🔍 You can now run tests with: ./test-setup.sh test"
        echo "🔄 Or restart services with: ./test-setup.sh restart"
        ;;
    "start-bg")
        start_services_background
        echo "✅ Background services started successfully!"
        echo "🔍 You can now run tests with: ./test-setup.sh test"
        echo "🛑 Stop background services with: ./test-setup.sh stop-bg"
        ;;
    "stop")
        stop_services
        echo "✅ Services stopped successfully!"
        ;;
    "stop-bg")
        stop_background_services
        ;;
    "status")
        if check_services; then
            echo "✅ All services are running!"
        else
            echo "❌ Some services are not running"
            exit 1
        fi
        ;;
    "test")
        run_tests
        ;;
    "start-and-test")
        start_and_test
        ;;
    "start-bg-and-test")
        start_bg_and_test
        ;;
    "restart")
        echo "🔄 Restarting services..."
        stop_services
        sleep 5
        start_services
        echo "✅ Services restarted successfully!"
        ;;
    "logs")
        if [ -f "$DOCKER_LOG" ]; then
            echo "📋 Showing Docker logs:"
            tail -f "$DOCKER_LOG"
        else
            echo "❌ No Docker log file found"
        fi
        ;;
    "help"|*)
        echo "Fleet Management Test Setup Script"
        echo ""
        echo "Usage: ./test-setup.sh [command]"
        echo ""
        echo "Commands:"
        echo "  start              - Start the full stack (client + server + database)"
        echo "  start-bg           - Start services in background"
        echo "  stop               - Stop all services"
        echo "  stop-bg            - Stop background services"
        echo "  status             - Check if all services are running"
        echo "  test               - Run E2E tests (requires services to be running)"
        echo "  start-and-test     - Start services, run tests, and cleanup (recommended)"
        echo "  start-bg-and-test  - Start background services, run tests, and cleanup"
        echo "  restart            - Restart all services"
        echo "  logs               - Show Docker logs (if using background mode)"
        echo "  help               - Show this help message"
        echo ""
        echo "Recommended workflow:"
        echo "  ./test-setup.sh start-and-test    # One command to run everything"
        echo ""
        echo "Alternative workflow:"
        echo "  1. ./test-setup.sh start-bg       # Start services in background"
        echo "  2. ./test-setup.sh test           # Run tests"
        echo "  3. ./test-setup.sh stop-bg        # Stop background services when done"
        ;;
esac 
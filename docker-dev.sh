#!/bin/bash

DEV_COMPOSE_FILE="docker-compose.dev.yml"
SQL_SERVER_VOLUME_NAME="capstone-project-25t2-3900-w18a-date_omnipulse_sql_data"

set -e

# Colours
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Colour

print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if .env file exists
check_env_file() {
    if [ ! -f ".env" ]; then
        print_warning ".env file not found. Creating from .env.example"
        if [ -f ".env.example" ]; then
            cp .env.example .env
            print_info "Created .env file. Please update it with your values."
        else
            print_error ".env.example file not found. Please create .env manually."
            exit 1
        fi
    fi
}

# Start development environment
start_dev() {
    print_info "Starting development environment"
    check_env_file
    docker-compose -f "$DEV_COMPOSE_FILE" up --build
}

# Start only client
start_client() {
    print_info "Starting client"
    docker-compose -f "$DEV_COMPOSE_FILE" up --build client-dev
}

# Start only server
start_server() {
    print_info "Starting server and database"
    docker-compose -f "$DEV_COMPOSE_FILE" up --build server-dev db
}

# Stop all services
stop_all() {
    print_info "Stopping all services"
    docker-compose -f "$DEV_COMPOSE_FILE" down
}

# Clean up
cleanup() {
    print_info "Cleaning up Docker resources"
    docker-compose -f "$DEV_COMPOSE_FILE" down -v
    docker system prune -f
}

# Delete SQL Server volume
delete_sql_data() {
    if docker volume inspect "$SQL_SERVER_VOLUME_NAME" >/dev/null 2>&1; then
        print_warning "Deleting Docker Volume: $SQL_SERVER_VOLUME_NAME"

        CONTAINERS_USING_VOLUME=$(docker ps -a -q --filter volume="$SQL_SERVER_VOLUME_NAME")

        print_info "Containers using the volume: $CONTAINERS_USING_VOLUME"

        if [ -n "$CONTAINERS_USING_VOLUME" ]; then
            print_info "Stopping and removing containers using the volume"
            docker stop $CONTAINERS_USING_VOLUME
            docker rm $CONTAINERS_USING_VOLUME
        fi

        docker volume rm "$SQL_SERVER_VOLUME_NAME"
        print_info "SQL data volume $SQL_SERVER_VOLUME_NAME deleted successfully."
    else
        print_info "Volume not found: $SQL_SERVER_VOLUME_NAME"
    fi
}

# Show logs
show_logs() {
    if [ -z "$1" ]; then
        docker-compose -f "$DEV_COMPOSE_FILE" logs -f
    else
        docker-compose -f "$DEV_COMPOSE_FILE" logs -f "$1"
    fi
}

# Rebuild specific service
rebuild() {
    if [ -z "$1" ]; then
        print_error "Please specify a service to rebuild (client-dev, server-dev, db)"
        exit 1
    fi
    print_info "Rebuilding $1"
    docker-compose -f "$DEV_COMPOSE_FILE" build --no-cache "$1"
}

# Main
case "$1" in
    "start"|"up")
        start_dev
        ;;
    "client")
        start_client
        ;;
    "server")
        start_server
        ;;
    "stop"|"down")
        stop_all
        ;;
    "cleanup"|"clean")
        cleanup
        ;;
    "delete-sql")
        delete_sql_data
        ;;
    "logs")
        show_logs "$2"
        ;;
    "rebuild")
        rebuild "$2"
        ;;
    "help"|"--help"|"-h")
        echo "Usage: $0 {start|client|server|stop|cleanup|logs|rebuild|help}"
        echo ""
        echo "Commands:"

        COMMANDS=(
            "start   - Start the full development environment"
            "client  - Start only the frontend client"
            "server  - Start only the backend server and database"
            "stop    - Stop all running services"
            "delete-sql - Delete SQL Server Docker volume"
            "cleanup - Stop services and clean up Docker resources"
            "logs    - Show logs for all services or specific service"
            "rebuild - Rebuild a specific service"
            "help    - Show this help message"
        )

        for cmd in "${COMMANDS[@]}"; do
            echo "  $cmd"
        done

        echo ""
        echo "Examples:"
        echo "  $0 start                    # Start everything"
        echo "  $0 client                   # Start only frontend"
        echo "  $0 logs client-dev          # Show frontend logs"
        echo "  $0 rebuild client-dev       # Rebuild frontend"
        ;;
    *)
        print_error "Unknown command: $1"
        echo "Use '$0 help' for usage information."
        exit 1
        ;;
esac
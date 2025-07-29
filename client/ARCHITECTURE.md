# Fleet Management Application - Architecture Guide

## Overview

This document outlines the architecture for the fleet management application, following modern React/Next.js best practices with a feature-based organization.

## Current Folder Structure

```
client/
├── src/
│   ├── app/                    # Next.js App Router pages (routing only)
│   │   ├── vehicles/
│   │   ├── work-orders/
│   │   ├── service-tasks/
│   │   ├── service-schedules/
│   │   ├── service-programs/
│   │   ├── contacts/
│   │   ├── issues/
│   │   ├── parts-inventory/
│   │   ├── settings/
│   │   └── layout.tsx
│   ├── components/             # Shared UI components
│   │   └── ui/                 # Base UI components (Button, Modal, Table, etc.)
│   ├── features/               # Feature-based modules
│   │   ├── vehicle/
│   │   ├── work-order/
│   │   ├── service-program/
│   │   ├── service-schedule/
│   │   ├── service-task/
│   │   ├── issue/
│   │   ├── inventory-item/
│   │   ├── vehicle-group/
│   │   ├── vehicle-status/
│   │   ├── technician/
│   │   └── user/
│   ├── hooks/                  # Global custom React hooks
│   ├── lib/                    # Third-party library configurations
│   ├── types/                  # Global TypeScript types
│   └── utils/                  # Utility functions
```

## Architecture Principles

### 1. Feature-Based Organization

Each feature is self-contained with its own:

- **components/**: UI components specific to the feature
- **hooks/**: Custom hooks for data fetching and mutations
- **api/**: API calls and external integrations
- **types/**: TypeScript interfaces and types
- **config/**: Configuration files (table columns, options, filters)
- **utils/**: Feature-specific utility functions
- **stores/**: Feature-specific state management (Zustand stores)

### 2. Separation of Concerns

- **App Router**: Only handles routing and page-level components
- **Features**: Business logic and domain-specific code
- **Components**: Reusable UI components
- **API Layer**: Data layer and external API calls
- **Stores**: Global state management

### 3. Data Flow

```
UI Components → Custom Hooks → API Services → Backend
     ↓              ↓              ↓
  Zustand Store → React Query → Axios Instance
```

## Feature Structure

```
features/vehicle/
├── components/
│   └── VehicleList.tsx
│   └── VehicleFormContainer.tsx
├── hooks/
│   └── useVehicles.ts          # Query and mutation hooks
├── api/
│   └── vehicleApi.ts           # API calls
├── types/
│   └── vehicle.types.ts        # TypeScript interfaces
├── config/
│   └── vehicleTableColumns.ts  # Table configuration
├── utils/
│   └── vehicleEnumHelper.ts    # Feature-specific utilities
└── stores/
    └── vehicleFormStore.ts     # Feature-specific state
```

## Component Organization

### UI Components (`src/components/ui/`)

Generic, reusable components organized by type:

- **Button/**: PrimaryButton, SecondaryButton, OptionButton
- **Detail/**: DetailFieldRow
- **Feedback/**: Loading, StatusBadge, EmptyState, Notification
- **Filter/**: Filter components
- **Form/**: Form components
- **Icons/**: Icon components
- **Layout/**: Layout components
- **Modal/**: Modal components
- **Table/**: Table components
- **Tabs/**: Tab components

### Feature Components (`src/features/{feature}/components/`)

Business-specific components organized by functionality:

- **list/**: List and table components
- **forms/**: Form and input components
- **detail/**: Detail view components

## State Management

- **React Query**: Server state (API data, caching, synchronization)
- **Zustand**: Client state (UI state, filters, selections)
- **Local State**: Component-specific state (form inputs, temporary UI state)

## API Layer

- Centralized axios instance with interceptors (`src/lib/axios.ts`)
- Service functions for each feature (`src/features/{feature}/api/`)
- Proper error handling and authentication
- Type-safe API responses

## Type Safety

- Define types close to where they're used
- Use strict TypeScript configuration
- Avoid `any` types, prefer proper interfaces
- Export types from feature modules

## Global Directories

### `src/hooks/`

Global custom hooks used across multiple features (e.g., `useDebounce.ts`)

### `src/lib/`

Third-party library configurations and instances

### `src/types/`

Global TypeScript types and interfaces (e.g., `pagedResponse.ts`)

### `src/utils/`

Utility functions used across the application (e.g., `dateTimeUtils.ts`)

## Benefits

1. **Scalability**: Easy to add new features without affecting existing code
2. **Maintainability**: Clear separation of concerns and responsibilities
3. **Reusability**: Components and hooks can be shared across features
4. **Type Safety**: Strong TypeScript integration throughout
5. **Performance**: Optimized data fetching and caching
6. **Developer Experience**: Clear file organization and predictable patterns
7. **Testing**: Easy to test individual features in isolation

# Fleet Management Application - Architecture Guide

## Overview

This document outlines the recommended architecture for the fleet management application, following modern React/Next.js best practices with a feature-based organization.

## Folder Structure

```
client/
├── src/
│   ├── app/                    # Next.js App Router pages (routing only)
│   │   ├── vehicles/
│   │   ├── work-orders/
│   │   ├── service-programs/
│   │   ├── issues/
│   │   ├── parts-inventory/
│   │   ├── settings/
│   │   └── layout.tsx
│   ├── components/             # Shared UI components
│   │   ├── ui/                 # Base UI components (Button, Modal, etc.)
│   │   ├── layout/             # Layout components (Header, Sidebar, etc.)
│   │   └── common/             # Business-specific shared components
│   ├── features/               # Feature-based modules
│   │   ├── vehicles/
│   │   ├── work-orders/
│   │   ├── service-programs/
│   │   ├── issues/
│   │   ├── inventory/
│   │   └── users/
│   ├── hooks/                  # Global custom React hooks
│   ├── lib/                    # Third-party library configurations
│   ├── stores/                 # State management (Zustand)
│   ├── types/                  # Global TypeScript types
│   ├── utils/                  # Utility functions
│   └── styles/                 # Global styles
```

## Architecture Principles

### 1. Feature-Based Organization

Each feature is self-contained with its own:

- **Components**: UI components specific to the feature
- **Hooks**: Custom hooks for data fetching and mutations
- **Services**: API calls and external integrations
- **Types**: TypeScript interfaces and types
- **Stores**: Local state management (if needed)

### 2. Separation of Concerns

- **App Router**: Only handles routing and page-level components
- **Features**: Business logic and domain-specific code
- **Components**: Reusable UI components
- **Services**: Data layer and external API calls
- **Stores**: Global state management

### 3. Data Flow Architecture

```
UI Components → Custom Hooks → API Services → Backend
     ↓              ↓              ↓
  Zustand Store → React Query → Axios Instance
```

## Feature Structure Example

```
features/vehicles/
├── components/
│   ├── VehicleList.tsx
│   ├── VehicleForm.tsx
│   ├── VehicleCard.tsx
│   └── VehicleFilters.tsx
├── hooks/
│   ├── useVehicles.ts          # Query hooks
│   └── useVehicleMutations.ts  # Mutation hooks
├── services/
│   └── vehicleApi.ts           # API calls
├── types/
│   └── vehicle.types.ts        # TypeScript interfaces
└── stores/
    └── vehicleStore.ts         # Local state (if needed)
```

## Best Practices

### 1. Component Organization

- **UI Components**: Generic, reusable components (Button, Modal, Table)
- **Feature Components**: Business-specific components (VehicleList, WorkOrderForm)
- **Page Components**: Route-level components (minimal logic, mostly composition)

### 2. State Management

- **React Query**: Server state (API data, caching, synchronization)
- **Zustand**: Client state (UI state, filters, selections)
- **Local State**: Component-specific state (form inputs, temporary UI state)

### 3. Type Safety

- Define types close to where they're used
- Use strict TypeScript configuration
- Avoid `any` types, prefer proper interfaces
- Export types from feature modules

### 4. API Layer

- Centralized axios instance with interceptors
- Service functions for each feature
- Proper error handling and authentication
- Type-safe API responses

### 5. Performance

- React Query for caching and background updates
- Code splitting by features
- Lazy loading of components
- Memoization where appropriate

## Migration Strategy

### Phase 1: Create New Structure

1. Create new folder structure
2. Move existing components to appropriate locations
3. Update imports and exports

### Phase 2: Refactor Components

1. Split large components into smaller, focused ones
2. Extract business logic into custom hooks
3. Implement proper TypeScript types

### Phase 3: Implement Data Layer

1. Create API services for each feature
2. Implement React Query hooks
3. Add proper error handling

### Phase 4: State Management

1. Implement Zustand stores for global state
2. Remove prop drilling
3. Optimize re-renders

## Code Examples

### Custom Hook Pattern

```typescript
// features/vehicles/hooks/useVehicles.ts
export const useVehicles = (filters?: VehicleFilters) => {
  return useQuery({
    queryKey: ["vehicles", filters],
    queryFn: () => vehicleApi.getVehicles(filters),
    staleTime: 5 * 60 * 1000,
  });
};
```

### Service Layer Pattern

```typescript
// features/vehicles/services/vehicleApi.ts
export const vehicleApi = {
  getVehicles: async (filters?: VehicleFilters): Promise<Vehicle[]> => {
    const response = await axiosInstance.get("/api/vehicles", {
      params: filters,
    });
    return response.data;
  },
  // ... other methods
};
```

### Component Pattern

```typescript
// features/vehicles/components/VehicleList.tsx
export const VehicleList: React.FC<VehicleListProps> = ({
  onEdit,
  onDelete,
}) => {
  const { data: vehicles, isLoading } = useVehicles();
  const { filters } = useVehicleStore();

  // Component logic here
};
```

## Benefits of This Architecture

1. **Scalability**: Easy to add new features without affecting existing code
2. **Maintainability**: Clear separation of concerns and responsibilities
3. **Reusability**: Components and hooks can be shared across features
4. **Type Safety**: Strong TypeScript integration throughout
5. **Performance**: Optimized data fetching and caching
6. **Developer Experience**: Clear file organization and predictable patterns
7. **Testing**: Easy to test individual features in isolation

## Next Steps

1. Review and approve this architecture
2. Create a migration plan
3. Start with one feature as a pilot
4. Gradually migrate existing code
5. Update documentation and team guidelines

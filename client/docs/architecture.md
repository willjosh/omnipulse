# Architecture Guide

This document outlines the key architectural decisions and patterns for the Fleet Management Application client.

## 🏗️ Core Principles

### 1. Feature-Based Organization

The application is organized around business features rather than technical concerns. Each feature is self-contained with its own:

- **Components**: UI components specific to the feature
- **Hooks**: Custom hooks for data and state management
- **API**: API calls and external integrations
- **Types**: TypeScript interfaces and types
- **Config**: Configuration files (table columns, options, filters)
- **Utils**: Feature-specific utility functions
- **State**: Feature-specific state management

### 2. Separation of Concerns

- **App Router**: Only handles routing and page-level components
- **Features**: Business logic and domain-specific code
- **Components**: Reusable UI components
- **API Layer**: Data layer and external API calls
- **State**: Global state management

### 3. Data Flow

```
UI Components → Custom Hooks → API Services → Backend
     ↓              ↓              ↓
   useState → React Query → Axios Instance
```

## 📁 Directory Structure

```
src/
├── app/                    # Next.js App Router pages (routing only)
│   ├── (auth)/            # Authentication routes
│   ├── vehicles/          # Vehicle management routes
│   ├── work-orders/       # Work order routes
│   ├── service-*/         # Service-related routes
│   ├── inventory/         # Inventory routes
│   ├── issues/            # Issue management routes
│   ├── settings/          # Application settings
│   └── layout.tsx         # Root layout
├── components/             # Shared UI components
│   ├── ui/                # Base UI component library
│   └── auth/              # Authentication components
├── features/               # Feature-based modules
│   ├── vehicle/           # Vehicle management
│   ├── work-order/        # Work order management
│   ├── service-program/   # Service program management
│   ├── service-schedule/  # Service scheduling
│   ├── service-task/      # Service task management
│   ├── service-reminder/  # Service reminders
│   ├── inventory/         # Inventory management
│   ├── inventory-item/    # Inventory item management
│   ├── inspection/        # Inspection management
│   ├── inspection-form/   # Inspection form management
│   ├── issue/             # Issue management
│   ├── maintenance-history/ # Maintenance history
│   ├── fuel-purchases/    # Fuel purchase tracking
│   ├── vehicle-group/     # Vehicle grouping
│   ├── vehicle-status/    # Vehicle status management
│   ├── auth/              # Authentication
│   ├── user/              # User management
│   └── technician/        # Technician management
├── hooks/                  # Global custom React hooks
├── lib/                    # Third-party library configurations
├── types/                  # Global TypeScript types
└── utils/                  # Utility functions
```

## 🎯 Feature Structure

Each feature follows a consistent structure:

````
features/{feature}/
├── components/                 # Feature-specific UI components
│   ├── {Feature}List.tsx      # List/table components
│   ├── {Feature}Form.tsx      # Form components
│   ├── {Feature}Detail.tsx    # Detail view components
│   └── {Feature}Card.tsx      # Card components
├── hooks/                      # Feature-specific custom hooks
│   ├── use{Feature}s.ts       # Query and mutation hooks
│   └── use{Feature}Form.ts    # Form state management
├── api/                        # API integration
│   └── {feature}Api.ts        # API calls and endpoints
├── types/                      # Feature-specific types
│   └── {feature}.types.ts     # TypeScript interfaces
├── config/                     # Configuration files
│   ├── {feature}TableColumns.ts # Table column definitions
│   └── {feature}Options.ts    # Dropdown options, filters
├── utils/                      # Feature-specific utilities
│   └── {feature}Helpers.ts    # Helper functions

## 🔄 State Management Strategy

The application uses a simple and effective state management approach:

### 1. React Query (Server State)
- **Purpose**: Manages server state, caching, and synchronization
- **Use Cases**: API data, pagination, real-time updates
- **Benefits**: Automatic caching, background updates, error handling

### 2. Local useState (Component State)
- **Purpose**: Manages component-specific and form state
- **Use Cases**: Form inputs, UI interactions, temporary data, form validation
- **Benefits**: Simple, built into React, no external dependencies, easy to understand and maintain

## 🌐 API Layer Architecture

### 1. Axios Instance

```typescript
// src/lib/axios.ts
const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
  timeout: 10000,
});

// Request/response interceptors for authentication and error handling
````

### 2. Feature API Services

```typescript
// src/features/vehicle/api/vehicleApi.ts
export const vehicleApi = {
  getVehicles: (params: VehicleQueryParams) =>
    api.get<Vehicle[]>("/vehicles", { params }),

  createVehicle: (data: CreateVehicleRequest) =>
    api.post<Vehicle>("/vehicles", data),

  updateVehicle: (id: string, data: UpdateVehicleRequest) =>
    api.put<Vehicle>(`/vehicles/${id}`, data),
};
```

### 3. React Query Integration

```typescript
// src/features/vehicle/hooks/useVehicles.ts
export const useVehicles = (params: VehicleQueryParams) => {
  return useQuery({
    queryKey: ["vehicles", params],
    queryFn: () => vehicleApi.getVehicles(params),
  });
};
```

## 🎨 Component Architecture

### 1. UI Component Library (`src/components/ui/`)

Generic, reusable components organized by functionality:

- **Button/**: PrimaryButton, SecondaryButton, IconButton
- **Form/**: FormField, FormSection, FormContainer
- **Table/**: DataTable, TableHeader, TableRow
- **Modal/**: Modal, ConfirmModal, FormModal
- **Feedback/**: Loading, StatusBadge, EmptyState, Notification
- **Layout/**: PageHeader, Sidebar, ContentArea

### 2. Authentication Components (`src/components/auth/`)

Authentication and authorization components:

- **ProtectedRoute**: Route protection component
- **AuthProvider**: Authentication context provider
- **LoginForm**: User login form
- **RegisterForm**: User registration form

### 3. Feature Components (`src/features/{feature}/components/`)

Business-specific components organized by functionality:

- **List Components**: Display data in tables, cards, or lists
- **Form Components**: Handle data input and validation
- **Detail Components**: Show detailed information
- **Card Components**: Compact information display

````

## 🔐 Authentication & Authorization

### 1. Protected Routes

```typescript
// src/components/auth/ProtectedRoute.tsx
export const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) return <Loading />;
  if (!isAuthenticated) return <Navigate to="/login" />;

  return <>{children}</>;
};
````

### 2. Route Groups

```typescript
// src/app/(auth)/layout.tsx
export default function AuthLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="auth-layout">
      {children}
    </div>
  );
}
```

## 🧪 Testing Strategy

### 1. Component Testing (Jest + React Testing Library)

- **Unit tests**: Individual component functionality
- **Integration tests**: Component interactions
- **Accessibility tests**: Screen reader and keyboard navigation

### 2. E2E Testing (Playwright)

- **User workflows**: Complete user journeys
- **Cross-browser**: Multiple browser testing
- **Visual regression**: UI consistency checks

### 3. Test Organization

```
client/src/__tests__/                    # Component tests
├── components/               # UI component tests
├── features/                 # Feature component tests
└── pages/                    # Page component tests

client/tests/                        # E2E tests
├── auth-flow.spec.ts         # Authentication workflows
├── data-management.spec.ts   # CRUD operations
└── form-interactions.spec.ts # Form handling
```

## 🚀 Performance Optimization

### 1. Code Splitting

- **Route-based**: Automatic with Next.js App Router
- **Component-based**: Dynamic imports for heavy components
- **Library-based**: Split large third-party libraries

### 2. Caching Strategy

- **React Query**: Server data caching
- **Next.js**: Static generation and ISR
- **Browser**: HTTP caching headers

### 3. Bundle Optimization

- **Tree shaking**: Remove unused code
- **Dynamic imports**: Load code on demand
- **Image optimization**: Next.js Image component

## 🔧 Development Workflow

### 1. Feature Development

1. **Create feature directory** with standard structure
2. **Define types** for data models
3. **Implement API layer** for backend communication
4. **Create components** for UI representation
5. **Add custom hooks** for business logic
6. **Write tests** for components and logic
7. **Update documentation** for new features

### 2. Component Development

1. **Design component interface** (props, events)
2. **Implement component logic** and state
3. **Add styling** with Tailwind CSS
4. **Write tests** for component behavior
5. **Add to component library** if reusable
6. **Update documentation** and examples

## 📊 Key Benefits

1. **Scalability**: Easy to add new features without affecting existing code
2. **Maintainability**: Clear separation of concerns and responsibilities
3. **Reusability**: Components and hooks can be shared across features
4. **Type Safety**: Strong TypeScript integration throughout
5. **Performance**: Optimized data fetching and caching
6. **Developer Experience**: Clear file organization and predictable patterns
7. **Testing**: Easy to test individual features in isolation
8. **State Management**: Simple React state with React Query for server state

---

_This architecture guide provides the foundation for building and maintaining the Fleet Management Application client. Follow these principles to ensure consistency and quality across the codebase._

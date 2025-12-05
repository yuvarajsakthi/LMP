# Project Structure

## Directory Organization

### Root Structure
```
LoanManagement/
├── Kanini.LMP/              # Main solution folder
│   ├── Kanini.LMP.Api/      # Web API layer
│   ├── Kanini.LMP.Application/  # Business logic layer
│   ├── Kanini.LMP.Data/     # Data access layer
│   ├── Kanini.LMP.Database/ # Database entities and DTOs
│   ├── Kanini.LMP.UI/       # React frontend
│   └── Kanini.LMP.UnitTests/  # Unit tests
├── .amazonq/                # Amazon Q configuration
└── README.md
```

## Core Components

### Backend Architecture (.NET 8.0)

#### Kanini.LMP.Api
**Purpose**: RESTful API layer exposing endpoints for frontend consumption

**Key Components**:
- **Controllers/**: API endpoints for different domains
  - CustomerController: Customer management operations
  - LoanApplicationController: Loan application CRUD
  - EligibilityController: Loan eligibility checks
  - EmiCalculatorController: EMI calculations
  - PaymentController: Payment processing
  - NotificationController: Notification management
  - TokenController: Authentication and authorization
- **Middleware/**: Global exception handling and request processing
- **Extensions/**: Dependency injection configuration
- **Constants/**: API-level constants and configuration
- **Program.cs**: Application entry point and service configuration

#### Kanini.LMP.Application
**Purpose**: Business logic and service implementations

**Key Components**:
- **Services/**: Business logic implementations
  - Implementations/: Concrete service classes
  - Interfaces/: Service contracts
- **Mappings/**: AutoMapper profiles for entity-DTO mapping
  - CustomerProfile, LoanProductProfile, NotificationProfile, PaymentTransactionProfile
- **Constants/**: Application-level constants and email templates
- **Extensions/**: Application layer dependency injection

#### Kanini.LMP.Data
**Purpose**: Data access layer with repository pattern and Entity Framework Core

**Key Components**:
- **Data/**: DbContext and database configuration
  - LMPDbContext: Main database context
  - LmpDbContextFactory: Factory for design-time operations
- **Repositories/**: Repository pattern implementation
  - Implementations/: Concrete repository classes
  - Interfaces/: Repository contracts
- **Migrations/**: Entity Framework migrations
- **UnitOfWork/**: Unit of Work pattern for transaction management
- **Scripts/**: SQL stored procedures and scripts
- **Extensions/**: Data layer dependency injection

#### Kanini.LMP.Database
**Purpose**: Database entities, DTOs, and enums

**Key Components**:
- **Entities/**: Domain entities
  - CustomerEntities/: Customer-related entities
  - LoanApplicationEntites/: Loan application entities
  - LoanProductEntities/: Loan product definitions
  - ManagerEntities/: Manager and approval entities
  - Notification, PaymentTransaction, User: Core entities
- **EntitiesDtos/**: Data Transfer Objects
  - Authentication/, CreditDtos/, CustomerEntitiesDtos/
  - LoanApplicationEntitiesDtos/, PaymentTransactionDtos/
  - ApiResponse: Standard API response wrapper
- **Enums/**: Enumeration types for status, types, and categories

### Frontend Architecture (React + TypeScript)

#### Kanini.LMP.UI
**Purpose**: Modern React-based user interface

**Key Components**:
- **src/components/**: Reusable UI components
- **src/pages/**: Page-level components and routes
- **src/services/**: API integration services
- **src/config/**: Configuration files
  - constants/endpoints.ts: API endpoint definitions
- **src/context/**: React context providers
- **src/guards/**: Route protection and authentication guards
- **src/hooks/**: Custom React hooks
- **src/layout/**: Layout components
- **src/middleware/**: Request/response interceptors
- **src/store/**: State management
- **src/types/**: TypeScript type definitions
- **src/utils/**: Utility functions

## Architectural Patterns

### Backend Patterns
1. **Layered Architecture**: Clear separation between API, Application, Data, and Database layers
2. **Repository Pattern**: Abstraction over data access logic
3. **Unit of Work Pattern**: Transaction management across repositories
4. **Dependency Injection**: Constructor-based DI throughout all layers
5. **DTO Pattern**: Separation of domain entities from API contracts
6. **AutoMapper**: Automated object-to-object mapping
7. **Middleware Pattern**: Cross-cutting concerns like exception handling

### Frontend Patterns
1. **Component-Based Architecture**: Modular, reusable React components
2. **Service Layer**: Centralized API communication
3. **Context API**: Global state management
4. **Route Guards**: Protected routes with authentication
5. **Custom Hooks**: Reusable stateful logic
6. **Configuration Management**: Centralized endpoint and constant management

## Component Relationships

### Data Flow
```
UI (React) 
  ↓ HTTP Requests
API Controllers 
  ↓ Service Calls
Application Services 
  ↓ Repository Calls
Data Repositories 
  ↓ EF Core
Database (SQL Server)
```

### Cross-Cutting Concerns
- **Authentication**: JWT-based authentication across API and UI
- **Logging**: Serilog for structured logging in API
- **Error Handling**: Global exception middleware in API, error boundaries in UI
- **Validation**: Data annotations in entities, form validation in UI
- **Notifications**: Email service integration for system notifications

## Technology Stack Integration
- **Backend**: .NET 8.0 Web API with Entity Framework Core 9.0
- **Frontend**: React 19 with TypeScript, Vite build tool
- **Database**: SQL Server with EF Core migrations
- **Authentication**: JWT Bearer tokens
- **Logging**: Serilog with file and console sinks
- **API Documentation**: Swagger/OpenAPI
- **UI Components**: Ant Design, React Hook Form, Recharts

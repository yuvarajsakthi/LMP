# Technology Stack

## Programming Languages

### Backend
- **C# 12** (.NET 8.0)
  - Target Framework: net8.0
  - Nullable reference types enabled
  - Implicit usings enabled

### Frontend
- **TypeScript** (~5.9.3)
  - Strict type checking
  - ES Module system
- **JavaScript/JSX/TSX**
  - React 19.2.0 with TypeScript

## Backend Technologies

### Core Framework
- **.NET 8.0 SDK**
  - ASP.NET Core Web API
  - Entity Framework Core 9.0.10

### Key NuGet Packages
- **AutoMapper.Extensions.Microsoft.DependencyInjection** (12.0.0) - Object-to-object mapping
- **Microsoft.AspNetCore.Authentication.JwtBearer** (8.0.10) - JWT authentication
- **Microsoft.AspNetCore.Mvc.NewtonsoftJson** (8.0.21) - JSON serialization
- **Microsoft.EntityFrameworkCore.SqlServer** (9.0.10) - SQL Server provider
- **Microsoft.EntityFrameworkCore.Tools** (9.0.10) - EF Core CLI tools
- **Serilog** (4.3.0) - Structured logging
- **Serilog.Extensions.Hosting** (9.0.0) - Serilog integration
- **Serilog.Sinks.Console** (6.0.0) - Console logging
- **Serilog.Sinks.File** (7.0.0) - File logging
- **Swashbuckle.AspNetCore** (6.6.2) - Swagger/OpenAPI documentation

### Database
- **SQL Server** with Entity Framework Core
- **Migrations**: Code-first approach with EF Core migrations

## Frontend Technologies

### Core Framework
- **React** (19.2.0)
- **React DOM** (19.2.0)
- **React Router DOM** (7.9.6) - Client-side routing

### Build Tools
- **Vite** (7.2.4) - Fast build tool and dev server
- **@vitejs/plugin-react** (5.1.1) - React plugin for Vite
- **TypeScript** (~5.9.3) - Type checking and compilation

### UI Libraries
- **Ant Design (antd)** (6.0.0) - UI component library
- **React Icons** (5.5.0) - Icon library
- **Framer Motion** (12.23.25) - Animation library
- **Recharts** (3.5.0) - Charting library

### State Management & Forms
- **React Hook Form** (7.67.0) - Form validation and management
- **Yup** (1.7.1) - Schema validation
- **React Auth Kit** (4.0.2-alpha.11) - Authentication management
- **Notistack** (3.0.2) - Notification/snackbar management

### HTTP & Utilities
- **Axios** (1.13.2) - HTTP client
- **jwt-decode** (4.0.0) - JWT token decoding
- **React Dropzone** (14.3.8) - File upload
- **React Error Boundary** (6.0.0) - Error handling

### Development Tools
- **ESLint** (9.39.1) - Code linting
- **@eslint/js** (9.39.1) - ESLint JavaScript config
- **eslint-plugin-react-hooks** (7.0.1) - React hooks linting
- **eslint-plugin-react-refresh** (0.4.24) - React refresh linting
- **typescript-eslint** (8.46.4) - TypeScript ESLint integration
- **@types/react** (19.2.5) - React type definitions
- **@types/react-dom** (19.2.3) - React DOM type definitions
- **@types/node** (24.10.1) - Node.js type definitions

## Development Commands

### Backend (.NET)
```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run API project
dotnet run --project Kanini.LMP/Kanini.LMP.Api

# Run tests
dotnet test

# Create migration
dotnet ef migrations add <MigrationName> --project Kanini.LMP.Data --startup-project Kanini.LMP.Api

# Update database
dotnet ef database update --project Kanini.LMP.Data --startup-project Kanini.LMP.Api
```

### Frontend (React + Vite)
```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run linter
npm run lint
```

## Configuration Files

### Backend
- **appsettings.json** - Main configuration
- **appsettings.Development.json** - Development environment config
- **appsettings.NotificationServices.json** - Notification service config
- **launchSettings.json** - Launch profiles and environment variables
- **.csproj files** - Project configuration and dependencies

### Frontend
- **package.json** - Dependencies and scripts
- **tsconfig.json** - TypeScript compiler configuration
- **tsconfig.app.json** - App-specific TypeScript config
- **tsconfig.node.json** - Node-specific TypeScript config
- **vite.config.ts** - Vite build configuration
- **eslint.config.js** - ESLint configuration
- **.env** - Environment variables

## Project Structure
- **Solution File**: Kanini.LMP.sln
- **Project Type**: Multi-project solution with layered architecture
- **Module System**: ES Modules (frontend), .NET assemblies (backend)

## Version Control
- **Git** with .gitignore for both .NET and Node.js artifacts

## API Documentation
- **Swagger UI** - Interactive API documentation available at runtime
- **OpenAPI Specification** - Auto-generated from API controllers

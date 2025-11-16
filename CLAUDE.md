# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET Aspire application built on .NET 10.0. .NET Aspire is an opinionated, cloud-ready stack for building observable, production-ready, distributed applications. The solution follows the standard Aspire architecture pattern with an AppHost orchestrator and a ServiceDefaults library.

## Project Structure

- `Demo.slnx` - Solution file at the root
- `aspire/Demo.Aspire.AppHost/` - Aspire AppHost orchestrator that defines the distributed application
- `aspire/Demo.Aspire.ServiceDefaults/` - Shared service defaults library containing common configurations
- `gateway/Demo.Gateway/` - API Gateway using YARP (Yet Another Reverse Proxy)
- `services/` - Backend microservices organized by domain (Identity, Organizations)
  - Each service follows a 3-layer architecture: Api, Domain, Infrastructure
- `apps/` - Frontend applications
  - `demo-app/` - Primary React application (Vite + TypeScript)
  - `demo-backoffice-app/` - Backoffice Vue.js application (Vite + TypeScript)

## Key Technologies

**Backend:**
- .NET 10.0
- .NET Aspire 13.0.0
- YARP 2.3.0 for API Gateway
- PostgreSQL (containerized)
- OpenTelemetry for observability (metrics, traces, logs)
- Serilog for structured logging
- Service discovery and resilience patterns

**Frontend:**

*demo-app (React):*
- React 19.2.0
- Vite 7.2.2
- TypeScript 5.9.3
- TailwindCSS 4.1.17 with Vite plugin
- Radix UI (headless component primitives)
- shadcn/ui (UI component system)
- Lucide React (icon library)
- ESLint for linting
- React Compiler (Babel plugin)
- CVA (Class Variance Authority) for component variants

*demo-backoffice-app (Vue.js):*
- Vue 3.5.13
- Vite 7.2.2
- TypeScript 5.9.3
- TailwindCSS 4.1.17 with Vite plugin
- Radix Vue (headless component primitives)
- shadcn-vue (UI component system)
- Lucide Vue Next (icon library)
- ESLint for linting
- CVA (Class Variance Authority) for component variants

## Common Commands

### Building and Running

```bash
# Run the entire distributed application (starts all services and frontends)
dotnet run --project aspire/Demo.Aspire.AppHost/Demo.Aspire.AppHost.csproj

# Build the entire solution
dotnet build Demo.slnx

# Restore .NET dependencies
dotnet restore Demo.slnx

# Build a specific service
dotnet build services/identity/Demo.Services.Identity.Api/Demo.Services.Identity.Api.csproj
dotnet build services/organizations/Demo.Services.Organizations.Api/Demo.Services.Organizations.Api.csproj
dotnet build gateway/Demo.Gateway/Demo.Gateway.csproj

# Run a specific service directly (without Aspire orchestration)
dotnet run --project services/identity/Demo.Services.Identity.Api/Demo.Services.Identity.Api.csproj
dotnet run --project gateway/Demo.Gateway/Demo.Gateway.csproj
```

### Frontend Development

```bash
# Install npm dependencies for a frontend app
cd apps/demo-app && npm install
cd apps/demo-backoffice-app && npm install

# Run frontend apps independently (without Aspire)
cd apps/demo-app && npm run dev
cd apps/demo-backoffice-app && npm run dev

# Build frontend apps for production
cd apps/demo-app && npm run build
cd apps/demo-backoffice-app && npm run build

# Lint frontend code
cd apps/demo-app && npm run lint
cd apps/demo-backoffice-app && npm run lint

# Preview production builds
cd apps/demo-app && npm run preview
cd apps/demo-backoffice-app && npm run preview

# Add shadcn/ui components (React - demo-app)
cd apps/demo-app
npx shadcn@latest add button
npx shadcn@latest add table

# Add shadcn-vue components (Vue - demo-backoffice-app)
cd apps/demo-backoffice-app
npx shadcn-vue@latest add button
npx shadcn-vue@latest add table
```

### Testing

```bash
# Run tests (when test projects are added)
dotnet test Demo.slnx
```

## Architecture

### AppHost (aspire/Demo.Aspire.AppHost/Program.cs)

The AppHost is the orchestrator for the distributed application using Aspire 13's JavaScript-first approach:

**Infrastructure Resources:**
- PostgreSQL container configuration is available but currently commented out
- When enabled: uses `postgres:latest` with persistent storage at `.containers/postgres`

**Frontend Applications:**
- `demo-app` - Primary React app using `AddViteApp()`
- `demo-backoffice-app` - Backoffice Vue.js app using `AddViteApp()`
- Both apps leverage Aspire 13's Vite-specific optimizations:
  - Automatic port binding (no manual `WithHttpEndpoint` needed)
  - Automatic npm/yarn/pnpm detection from package.json
  - Automatic "dev" script execution during development
  - Automatic "build" script execution during publishing
  - Automatic Dockerfile generation (no `PublishAsDockerFile()` needed)
  - Hot Module Replacement (HMR) support
  - Disable browser auto-open (`BROWSER=none`)
  - Configured for external HTTP access via `WithExternalHttpEndpoints()`

Key patterns:
- Uses `DistributedApplication.CreateBuilder(args)` to create the app
- Resources are defined using fluent builder pattern
- `AddViteApp()` replaces the deprecated `AddNpmApp()` with enhanced Vite support
- Package: `Aspire.Hosting.JavaScript` v13.0.0 (replaces `Aspire.Hosting.NodeJs`)

### API Gateway (gateway/Demo.Gateway/Program.cs)

A centralized API Gateway built with YARP (Yet Another Reverse Proxy) 2.3.0:

**Key Features:**
- Reverse proxy for routing requests to backend services
- Configuration-based routing via `ReverseProxy` section in appsettings
- Loads proxy configuration from `builder.Configuration.GetSection("ReverseProxy")`
- Maps routes using `app.MapReverseProxy()`

**Usage:**
The gateway sits between frontend applications and backend services, providing a single entry point for API requests and enabling cross-cutting concerns like routing, load balancing, and request transformation.

### ServiceDefaults (aspire/Demo.Aspire.ServiceDefaults/Extensions.cs)

The ServiceDefaults library provides common cross-cutting concerns for all services:

**Observability**:
- Serilog for structured logging (console + optional OTLP export)
- OpenTelemetry for distributed tracing and metrics
- Automatic instrumentation for ASP.NET Core, HTTP clients, and runtime metrics

**Resilience**:
- Standard resilience handler for HTTP clients (retry, circuit breaker, timeout)
- Service discovery integration

**Health Checks**:
- `/health` endpoint for overall health
- `/alive` endpoint for liveness probes (tagged with "live")

**Usage Pattern**:
Services should call `builder.AddServiceDefaults()` during startup and `app.MapDefaultEndpoints()` to register health check endpoints.

### Backend Services Architecture

The solution follows a domain-driven design approach with services organized by business domain:

**Identity Service** (`services/identity/`):
- Api layer: ASP.NET Core Web API
- Domain layer: Business logic and entities
- Infrastructure layer: Data access and external integrations

**Organizations Service** (`services/organizations/`):
- Api layer: ASP.NET Core Web API
- Domain layer: Business logic and entities (implements DDD building blocks)
  - `Organization`: Aggregate root managing locations and employees
  - `OrganizationLocation`: Entity representing physical locations
  - `OrganizationEmployee`: Entity representing employees assigned to locations
  - `OrganizationId`, `OrganizationLocationId`, `OrganizationEmployeeId`: Strongly-typed IDs
- Infrastructure layer: Data access and external integrations

**Domain Layer Patterns**:

The Organizations service implements tactical DDD patterns in `Demo.Services.Organizations.Domain/Abstractions/`:

- **Entity<TId>**: Base class for domain entities with identity-based equality
  - Implements `IEquatable<Entity<TId>>` with ID-based comparison
  - Includes operator overloads for equality checks
  - Hash code based on both type and ID

- **AggregateRoot<TId>**: Extends Entity to mark aggregate boundaries
  - Inherits from `Entity<TId>` and implements `IAggregateRoot`
  - Defines transaction and consistency boundaries

- **ValueObject**: Base class for immutable value objects
  - Implements structural equality via `GetEqualityComponents()`
  - Derived classes override `GetEqualityComponents()` to define equality
  - Immutable by convention (use private setters or init-only properties)

- **CompositeEntity**: For child entities within an aggregate that need both parent reference and own identity

Example domain entity pattern (see `Organizations/Organization.cs`):
```csharp
public sealed class Organization : AggregateRoot<OrganizationId>
{
    private readonly List<OrganizationLocation> _locations = [];
    private readonly List<OrganizationEmployee> _employees = [];

    private Organization() { } // EF Core constructor
    private Organization(OrganizationId id, string name) : base(id)
    {
        Name = name;
        Status = OrganizationStatus.Inactive;
    }

    public string Name { get; private set; } = null!;
    public OrganizationStatus Status { get; private set; }
    public IReadOnlyList<OrganizationLocation> Locations => _locations.AsReadOnly();
    public IReadOnlyList<OrganizationEmployee> Employees => _employees.AsReadOnly();

    // Factory method for creating organization from backoffice
    public static Organization CreateFromBackoffice(
        string organizationName,
        List<OrganizationEmployeeData> organizationAdmins)
    {
        var organization = new Organization(OrganizationId.Create(), organizationName);
        var location = OrganizationLocation.CreateDefault(organization.Id, organizationName);
        organization._locations.Add(location);

        foreach (var admin in organizationAdmins)
        {
            var employee = OrganizationEmployee.CreateAdmin(
                organization.Id,
                location.Id,
                admin.FirstName,
                admin.LastName,
                admin.Email,
                admin.Phone);
            organization._employees.Add(employee);
        }
        return organization;
    }

    // Method for updating organization from backoffice
    public void UpdateFromBackoffice(string name, OrganizationStatus status)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name is required");
        Name = name;
        Status = status;
    }
}
```

Key patterns demonstrated:
- Factory methods for different creation contexts (`CreateFromBackoffice`)
- Employees are automatically assigned to the default location during creation
- Aggregate root controls all modifications to child entities
- Backoffice operations are limited to name and status updates

**Common patterns across services:**
- All service projects target .NET 10.0
- `TreatWarningsAsErrors` is enabled
- Services should integrate ServiceDefaults for observability and resilience
- Each service can be run independently or through the AppHost orchestrator
- Domain entities use strongly-typed IDs (e.g., `OrganizationId` instead of `Guid`)
- Aggregate roots expose collections as `IReadOnlyList<T>` backed by private fields
- Entity creation uses static factory methods (e.g., `Create()`) rather than public constructors

### Frontend Architecture

**demo-app (React):**
- Built with Vite for fast development and optimized production builds
- TypeScript for type safety
- React 19 with React Compiler for optimized rendering
- ESLint for code quality
- TailwindCSS for styling with utility-first approach
- shadcn/ui components in `src/components/ui/`
- Path aliases: `@/*` maps to `./src/*` (configured in tsconfig.json and tsconfig.app.json)
- Standard Vite project structure with `src/` directory
- Multi-stage Docker builds (Node 24 build → Nginx Alpine runtime)

**UI Component Stack (React):**
- **shadcn/ui**: Copy-paste component system built on Radix UI primitives
- **Radix UI**: Unstyled, accessible component primitives
- **TailwindCSS**: Utility-first CSS framework for styling
- **CVA**: Type-safe component variants
- **Lucide React**: Icon library
- **tailwind-merge & clsx**: Utility class management

**demo-backoffice-app (Vue.js):**
- Built with Vite for fast development and optimized production builds
- TypeScript for type safety
- Vue 3.5 with Composition API (`<script setup>`)
- ESLint for code quality
- TailwindCSS for styling with utility-first approach
- shadcn-vue components in `src/components/ui/`
- Path aliases: `@/*` maps to `./src/*` (configured in tsconfig.json and tsconfig.app.json)
- Standard Vite project structure with `src/` directory
- Hash-based routing for navigation (`#organizations`, `#employees/:id`)

**UI Component Stack (Vue):**
- **shadcn-vue**: Copy-paste component system built on Radix Vue primitives
- **Radix Vue**: Unstyled, accessible component primitives
- **TailwindCSS**: Utility-first CSS framework for styling
- **CVA**: Type-safe component variants
- **Lucide Vue Next**: Icon library
- **tailwind-merge & clsx**: Utility class management

**Backoffice Application Structure:**
- `src/components/organizations/` - Organization management components
  - `OrganizationsList.vue` - Main list view with create/edit capabilities
  - `OrganizationFormSheet.vue` - Side sheet for creating/editing organizations
  - `OrganizationEmployeesList.vue` - Employee list for specific organization
- `src/components/ui/` - shadcn-vue UI components (Table, Button, Badge, Sheet, Dropdown, etc.)
- `src/types/organization.ts` - TypeScript types matching backend domain
- `src/data/mock-organizations.ts` - Mock data for development
- `src/lib/organization-utils.ts` - Helper functions (formatters, status labels)

**Backoffice Routing:**
- `#organizations` - Main organizations list
- `#employees/:organizationId` - Employees for specific organization
- Breadcrumbs show navigation path (Backoffice > Organizations > [Org Name] > Employees)

**Backoffice vs Owner Responsibilities:**

*Backoffice Admin (demo-backoffice-app) can:*
- Create organizations with initial administrators
- Update organization name and status (Inactive/Active/Archived)
- View all employees across organizations (read-only)
- View organization locations (read-only)

*Organization Owner (demo-app) can:*
- Manage locations (add, edit, delete, set opening hours)
- Manage employees (invite, edit roles, remove)
- Edit organization details within their organization
- Manage day-to-day operations

This separation ensures system administrators handle organization onboarding and status management, while organization owners control their own operational data.

**Import Convention (both apps):**
Use the `@/` alias for all internal imports to maintain clean import paths:
```typescript
// React (demo-app)
import { Button } from "@/components/ui/button"
import { utils } from "@/lib/utils"

// Vue (demo-backoffice-app)
import { Button } from '@/components/ui/button'
import { formatDate } from '@/lib/organization-utils'
```

## Development Notes

- Warnings are treated as errors (`TreatWarningsAsErrors` is enabled) across all .NET projects
- User secrets are configured for the AppHost (ID: 087c8d24-895e-48f1-90b0-3b51d0c8e43f)
- OpenTelemetry exports are conditionally enabled based on `OTEL_EXPORTER_OTLP_ENDPOINT` configuration
- Health check endpoints are only mapped in Development environment
- PostgreSQL data is persisted to `.containers/postgres` directory (ignored by git)
- When running through Aspire AppHost, all services and frontends start together with proper service discovery

## Naming Conventions

**Backend Projects:**
- Pattern: `Demo.{Area}.{Component}.{Layer}`
- Examples:
  - `Demo.Services.Identity.Api`
  - `Demo.Services.Organizations.Domain`
  - `Demo.Aspire.AppHost`
  - `Demo.Gateway`

**Frontend Projects:**
- Pattern: kebab-case
- Examples: `demo-app`, `demo-backoffice-app`

**Path Aliases:**
- Both frontend projects use `@/*` to reference `./src/*`
- React (demo-app): configured in `tsconfig.json` and `tsconfig.app.json`
- Vue (demo-backoffice-app): configured in `tsconfig.json`, `tsconfig.app.json`, and `vite.config.ts`
- Each project's alias is scoped to its own `src/` directory (no cross-project imports)
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
  - `demo-backoffice-app/` - Backoffice React application (Vite + TypeScript)

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
- React 19.2.0
- Vite 7.2.2
- TypeScript 5.9.3
- ESLint for linting
- React Compiler (Babel plugin)

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
- PostgreSQL container (`demo-postgres`) with persistent storage at `.containers/postgres`
- Uses `postgres:latest` image
- Container lifetime set to `Persistent` to preserve data across restarts

**Frontend Applications:**
- `demo-app` - Primary React app using `AddViteApp()`
- `demo-backoffice-app` - Backoffice React app using `AddViteApp()`
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
- Domain layer: Business logic and entities
- Infrastructure layer: Data access and external integrations

**Common patterns across services:**
- All service projects target .NET 10.0
- `TreatWarningsAsErrors` is enabled
- Services should integrate ServiceDefaults for observability and resilience
- Each service can be run independently or through the AppHost orchestrator

### Frontend Architecture

Both React applications follow the same structure:
- Built with Vite for fast development and optimized production builds
- TypeScript for type safety
- React 19 with React Compiler for optimized rendering
- ESLint for code quality
- Standard Vite project structure with `src/` directory

## Development Notes

- Warnings are treated as errors (`TreatWarningsAsErrors` is enabled) across all .NET projects
- User secrets are configured for the AppHost (ID: 087c8d24-895e-48f1-90b0-3b51d0c8e43f)
- OpenTelemetry exports are conditionally enabled based on `OTEL_EXPORTER_OTLP_ENDPOINT` configuration
- Health check endpoints are only mapped in Development environment
- PostgreSQL data is persisted to `.containers/postgres` directory (ignored by git)
- When running through Aspire AppHost, all services and frontends start together with proper service discovery
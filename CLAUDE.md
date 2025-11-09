# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET Aspire application built on .NET 9.0. .NET Aspire is an opinionated, cloud-ready stack for building observable, production-ready, distributed applications. The solution follows the standard Aspire architecture pattern with an AppHost orchestrator and a ServiceDefaults library.

## Project Structure

- `Demo.slnx` - Solution file at the root
- `aspire/Demo.AppHost/` - Aspire AppHost orchestrator that defines the distributed application
- `aspire/Demo.ServiceDefaults/` - Shared service defaults library containing common configurations

## Key Technologies

- .NET 9.0
- .NET Aspire 9.5.2
- PostgreSQL (containerized)
- OpenTelemetry for observability (metrics, traces, logs)
- Serilog for structured logging
- Service discovery and resilience patterns

## Common Commands

### Building and Running

```bash
# Run the AppHost (this starts the entire distributed application)
dotnet run --project aspire/Demo.AppHost/Demo.AppHost.csproj

# Build the solution
dotnet build Demo.slnx

# Restore dependencies
dotnet restore Demo.slnx
```

### Testing

```bash
# Run tests (when test projects are added)
dotnet test Demo.slnx
```

## Architecture

### AppHost (aspire/Demo.AppHost/Program.cs)

The AppHost is the orchestrator for the distributed application. It:
- Defines all resources (databases, services, containers)
- Configures service dependencies and connections
- Currently defines a PostgreSQL container with persistent storage at `.containers/postgres`

Key patterns:
- Uses `DistributedApplication.CreateBuilder(args)` to create the app
- Resources are defined using fluent builder pattern (e.g., `AddPostgres()`, `AddDatabase()`)
- Container lifetime is set to `Persistent` for the PostgreSQL instance
- The PostgreSQL database is named "demo-db" and accessible via service name "demo-postgres"

### ServiceDefaults (aspire/Demo.ServiceDefaults/Extensions.cs)

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

## Development Notes

- Warnings are treated as errors (`TreatWarningsAsErrors` is enabled)
- User secrets are configured for the AppHost (ID: 087c8d24-895e-48f1-90b0-3b51d0c8e43f)
- OpenTelemetry exports are conditionally enabled based on `OTEL_EXPORTER_OTLP_ENDPOINT` configuration
- Health check endpoints are only mapped in Development environment
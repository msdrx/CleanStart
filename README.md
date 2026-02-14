# CleanStart

> **Beta:** This project is under active development and may change.

A `dotnet new` template that scaffolds a Clean Architecture solution for ASP.NET Core Web API.

The generated solution follows Clean Architecture principles — giving you a production-ready project structure out of the box.

## Installation

```bash
dotnet new install CleanStart.Templates
```

## Usage

```bash
# Create a new project
dotnet new cleanstart -n MyProject -o ./MyProject

# Target a specific .NET version
dotnet new cleanstart -n MyProject --framework net10.0
```

### Parameters

| Parameter | Values | Default |
|---|---|---|
| `--framework` | `net6.0`, `net8.0`, `net10.0` | `net8.0` |
| `--skipRestore` | `true`, `false` | `false` |

## What You Get

A 10-project solution organized into 4 layers:

- **Apps** — ASP.NET Core host, controllers, Swagger
- **Core** — Domain entities, repository interfaces, service contracts (zero infrastructure dependencies)
- **Infrastructure** — EF Core + PostgreSQL, HttpClient abstractions, EF migrations host
- **Shared** — Exceptions, constants, middleware, API DTOs

Key features included:
- Generic Repository with soft-delete support and paged queries
- Global error handling middleware
- Swagger with XML documentation
- DI extension methods per layer

## License

MIT

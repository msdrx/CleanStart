# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Project Is

CleanStart is a **dotnet new template** that scaffolds a Clean Architecture solution for ASP.NET Core Web APIs targeting PostgreSQL. The repository contains template source(s) under the `templates/` directory. All projects, naming, and ports use the placeholder name `CleanStart` which the template engine replaces via `sourceName`.

## Repository Layout

```
/
├── templates/
│   └── cleanstart-api/                 ← Clean Architecture template source
│       ├── .template.config/
│       ├── .editorconfig
│       ├── CleanStart.sln
│       ├── CleanStart.Flat.sln
│       └── src/
├── CleanStart.Template.csproj          ← NuGet packaging (globs templates/**)
├── README.md
├── CLAUDE.md
├── specs/
└── TemplatingDocs/
```

New templates are added by creating a directory under `templates/` with its own `.template.config/template.json`. No changes to the packaging project are needed.

## Build & Run Commands

```bash
# Build (use the nested layout solution)
dotnet build templates/cleanstart-api/CleanStart.sln

# Run the API (launches on http://localhost:5211, https://localhost:7184)
dotnet run --project templates/cleanstart-api/src/Apps/CleanStart.Api

# EF Core migrations (must run from the Design project)
dotnet ef migrations add <Name> --project templates/cleanstart-api/src/Infrastructure/CleanStart.Infrastructure.Design

# Template operations
dotnet new install .                                    # Install template locally
dotnet new cleanstart -n MyProject -o ./output          # Scaffold a new project
dotnet new cleanstart -n MyProject --framework net9.0   # Scaffold with .NET 9
dotnet new uninstall .                                  # Uninstall template
```

There are no test projects in this solution.

## Solution Structure (10 projects, 4 layers)

Two solution files exist: `CleanStart.sln` (nested under `src/`) and `CleanStart.Flat.sln` (flat layout). The template conditionally picks one based on the `includeSrcFolder` computed symbol (default: flat).

### Dependency flow (top → bottom)

```
CleanStart.Api
  └─ CleanStart.Shared.Api  (composition root: wires all DI, middleware, NLog)
       ├─ CleanStart.Application
       │    ├─ CleanStart.Domain  (includes repository interfaces in Repositories/Base/) → CleanStart.Shared
       │    └─ CleanStart.Service.Abstractions → CleanStart.Domain.Models → CleanStart.Domain
       ├─ CleanStart.Infrastructure.Persistence  (EF Core + Npgsql)
       └─ CleanStart.Infrastructure.Service      (HttpClient abstractions)

CleanStart.Infrastructure.Design  (standalone entry point for EF migrations only)
  └─ CleanStart.Infrastructure.Persistence
```

**Core** projects (Domain, Application) have zero infrastructure dependencies. Infrastructure implements the abstractions.

### Layer responsibilities

- **Apps/CleanStart.Api** — ASP.NET Core host, controllers, Swagger. XML docs output to `src/Apps/SwaggerDocs/`.
- **Core/** — Domain entities (`BaseEntity<TKey>`, `BaseSoftDeleteEntity<TKey>`), repository/service interfaces, application service DI registration.
- **Infrastructure/Persistence** — `CleanStartDbContext` (PostgreSQL, schema `"Core"`, uppercase table/column names, cascade delete disabled), `GenericRepository<TContext, TEntity>`, EF configurations with global soft-delete query filter.
- **Infrastructure/Service** — `GenericHttpClient` (via `IHttpClientFactory`), correlation ID propagation, minimal HTTP client logging.
- **Infrastructure/Design** — Design-time web host solely for `dotnet ef` tooling. Connection string and migrations assembly configured here.
- **Shared/** — Cross-cutting: exception hierarchy (`CleanStartException` → `DomainException`, `ObjectNullException`), `ExceptionCode` enum, constants.
- **Shared.Api** — API-layer cross-cutting: `ErrorMiddleware` (global exception → HTTP status mapping), `RequestContextMiddleware` (correlation ID via `X-Correlation-Id`), `ApiResponse<T>`, `GridRequest`/`GridResponse<T>`.

## Key Architectural Patterns

- **Generic Repository** with `AsNoTracking()` default, soft-delete support, paged queries, and entity detach after save.
- **Soft Delete** via `RecordStatus` enum + global query filter (`HasQueryFilter`). Bypassable with `ignoreGlobalQueryFilters` parameter.
- **DI Extension Methods** per layer: `AddApplicationServices()`, `AddDatabase()`, `AddRepositories()`, `AddMinimalHttpClientLogging()`, `AddRequestContextProvider()`.
- **Middleware pipeline order**: ErrorMiddleware → RequestContextMiddleware → Swagger → Routing → HTTPS Redirect → Authorization → MapControllers.
- **Base entity ID generation**: `ValueGeneratedNever` for Guid/Enum keys, `ValueGeneratedOnAdd` for others (configured in `BaseEntityConfiguration`).

## Template Engine Details

Template config is in `templates/cleanstart-api/.template.config/template.json`. Key behaviors:

- `sourceName: "CleanStart"` — replaced in all file contents, filenames, and directory names when scaffolding.
- Parameters: `Framework` (net8.0/net9.0), `skipRestore` (--no-restore).
- Ports (5211, 7184, 60626, 44394) are replaced with randomly generated values.
- 16 project GUIDs are regenerated per instantiation.
- Template excludes: `.template.config`, build artifacts, IDE files, `src/**` (root-level source block).

## Database

PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL`. Default connection string in appsettings: `Host=localhost;Port=5432;Database=CleanStart;Username=admin;Password=test`.

## EditorConfig

- 2-space indentation, CRLF line endings, UTF-8.
- `IDE0130` (namespace must match folder): **error**.
- `CS1591` (missing XML doc comments): suppressed.

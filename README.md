# UBB-SE-2026-832-1

## Solution Structure

- `ClassLibrary`
  - `Models` - shared domain models
  - `DTOs` - transport contracts
  - `IRepositories` - repository interfaces
  - `Repositories` - repository implementations
  - `Data` - EF Core `AppDbContext`
  - `Extensions` - registration and initialization helpers:
    - `ServiceCollectionExtensions` for DI registrations
    - `DatabaseInitializer` for seed data
- `WebAPI`
  - `Controllers` - HTTP endpoints
  - `Services` - business logic layer
  - `Properties/launchSettings.json` - run profiles (Development + Swagger)
- `WinUI`
  - `Views` - XAML pages/windows
  - `Viewmodels` - MVVM view models
  - `Services` - UI services and proxy services for API calls

## Architecture Rules

- Keep business logic in services.
- Keep model definitions in `ClassLibrary/Models`; do not redefine them in `WinUI`.
- Keep repository interfaces in `ClassLibrary/IRepositories`.
- Keep repository implementations in `ClassLibrary/Repositories`.
- Keep EF context definitions in `ClassLibrary/Data`.
- Model object links via class references, not foreign-key id fields.
- `WebAPI` should call extension methods for data-layer wiring, not reference data-layer internals directly.
- `WinUI` communicates with backend through services/proxies, not direct database access.
- `WinUI` proxy services must receive all dependencies via constructor injection (e.g., `HttpClient`, config, auth handlers) and avoid instantiating dependencies inside the proxy.

## Prerequisites

- Windows + Visual Studio 2022
- .NET 8 SDK

## Getting Started

1. Open `UBB-SE-2026-832-1.sln`.
2. Restore packages (right click solution -> Restore NuGet Packages).
3. Build solution.
4. Set `WebAPI` as startup project for backend testing.

## Running and Testing WebAPI

`WebAPI/Properties/launchSettings.json` is configured with Development environment and Swagger launch.

When running `WebAPI`, test:

- Swagger UI: `http://localhost:5066/swagger`
- Users endpoint: `http://localhost:5066/api/users`

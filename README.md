# UBB-SE-2026-832-1

Team repository for the merge phase (ClassLibrary + WebAPI + WinUI) in Software Engineering.

## Solution Structure

- `ClassLibrary`
  - `Models` - shared domain/data models
  - `DTOs` - transport contracts
  - `IRepositories` - repository interfaces (contracts only)
- `WebAPI`
  - `Controllers` - HTTP endpoints
  - `Services` - business/service layer
  - `Repositories` - data access implementations
  - `Properties/launchSettings.json` - run profiles (Development + Swagger)
- `WinUI`
  - `Views` - XAML pages/windows
  - `Viewmodels` - MVVM view models
  - `Services` - UI services and proxy services for API calls

## Architecture Rules

- Keep business logic out of GUI code (`Views` / code-behind).
- Keep model definitions in `ClassLibrary/Models`; do not redefine models in `WinUI`.
- Keep repository interfaces in `ClassLibrary/IRepositories`.
- Keep repository implementations in `WebAPI/Repositories`.
- `WinUI` communicates with backend through services/proxies, not direct DB access.

## Prerequisites

- Windows + Visual Studio 2022 (for WinUI project)
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

Notes:

- `404` on `/` is normal unless a root endpoint is mapped.
- `404` on `/favicon.ico` is normal if no icon file is provided.


# HomeScreen

HomeScreen is an automation platform for displaying an image slideshow on low-powered devices such as the Raspberry Pi. It is composed of .NET services and web UIs orchestrated with .NET Aspire. The system ingests media, enriches it with location and weather data, and serves slideshow and dashboard experiences to lightweight devices.

## Overview

This repository is a multi-project solution with:
- An Aspire AppHost that orchestrates local development resources (Redis, SQL Server) and services
- Backend services (Media, Location, Weather) built with ASP.NET Core (gRPC/HTTP), targeting .NET 9
- Web servers (Common, Dashboard, Slideshow) in ASP.NET Core serving Vue 3 clients
- Front-end projects built with Vite + Vue 3 + TypeScript, managed with pnpm
- Testing projects for services and integration tests using xUnit and Aspire.Hosting.Testing; front-ends use Vitest

## Tech Stack

- Languages: C#, TypeScript
- Frameworks/Runtime:
  - .NET 9 (global.json sets SDK 9)
  - ASP.NET Core (Minimal API, gRPC clients, NSwag for OpenAPI)
  - .NET Aspire (orchestration via AppHost)
  - Vue 3 + Vite + TypeScript (web clients)
- Data/Infra (via Aspire during dev):
  - Redis (with RedisInsight)
  - SQL Server (Microsoft mssql/server container)
- Tooling: pnpm (Node >= 22), Vitest, Storybook, TailwindCSS/DaisyUI, Biome

## Requirements

- .NET SDK 9 (see global.json)
- Node.js >= 22
- pnpm >= 10
- Docker Desktop (for Aspire to provision Redis and SQL Server containers)
- Optional: Azure Maps credentials (if using Azure as MappingService)

## Project Structure

Top-level directories:
- HomeScreen.AppHost — .NET Aspire AppHost entry point that wires all services and resources
- HomeScreen.Database — database migrations (e.g., MediaDb.Migrations)
- HomeScreen.Integration.Tests — integration tests using Aspire testing
- HomeScreen.ServiceDefaults — shared service defaults for ASP.NET services
- HomeScreen.Services — backend services
  - HomeScreen.Service.Location — location service and Nominatim OpenAPI client
  - HomeScreen.Service.Media — media API and background worker
  - HomeScreen.Service.Weather — weather service
- HomeScreen.Web — web servers and front-end clients
  - HomeScreen.Web.Common — shared server and component libraries
    - HomeScreen.Web.Common.Server — ASP.NET Core server exposing common APIs
    - homescreen.web.common.components — Vue component library (Vite)
    - homescreen.web.common.components.api — generated API helpers for web
  - HomeScreen.Web.Dashboard — ASP.NET Core server plus dashboard client
    - homescreen.web.dashboard.client — Vue app (Vite)
  - HomeScreen.Web.Slideshow — ASP.NET Core server plus slideshow client
    - homescreen.web.slideshow.client — Vue app (Vite)

## Entry Points

- Aspire AppHost: HomeScreen.AppHost/Program.cs (primary entry for local dev; provisions Redis/SQL and starts services)
- Services:
  - HomeScreen.Service.Media/Program.cs (media HTTP/gRPC service)
  - HomeScreen.Service.Media.Worker/Program.cs (background worker)
  - HomeScreen.Service.Location/Program.cs (location service)
  - HomeScreen.Service.Weather/Program.cs (weather service)
  - HomeScreen.Web.Common.Server/Program.cs (common API)
  - HomeScreen.Web.Dashboard.Server/Program.cs (dashboard server)
  - HomeScreen.Web.Slideshow.Server/Program.cs (slideshow server)

## Environment Variables / Parameters

Configured in AppHost (DistributedApplication builder). Some are parameters you supply at run-time:
- MappingService: Nominatim | Azure | Blank (default shown in code: Nominatim)
- HomeScreenSqlServerPassword: password for the local SQL Server container
- MediaSourceDir: host path containing your source media (mounted into the media worker/service)
- MediaCacheDir: host path for cached/processed media
- AZURE_MAPS_SUBSCRIPTION_KEY: Azure Maps API key (required if MappingService=Azure)
- AZURE_CLIENT_ID: Azure AD application client ID (used by mapping integrations)
- CommonAddress: internal address of Common server (injected into dashboard/slideshow)
- DashboardAddress: internal address of Dashboard server (injected into slideshow)
- SlideshowAddress: internal address of Slideshow server (injected into dashboard)

Notes:
- AppHost config also defines volume mounts named "cache" and "media" to /cache and /media for media service/worker.
- AppHost automatically wires Redis and SQL Server; see Program.cs in HomeScreen.AppHost for details.

## Setup

1) Install prerequisites
- Install .NET 9 SDK
- Install Node.js 22+
- Install pnpm 10+
- Ensure Docker Desktop is running

2) Install front-end dependencies
- The web clients and components use pnpm. From each front-end directory, run pnpm install:
  - HomeScreen.Web/HomeScreen.Web.Common/homescreen.web.common.components
  - HomeScreen.Web/HomeScreen.Web.Common/homescreen.web.common.components.api
  - HomeScreen.Web/HomeScreen.Web.Dashboard/homescreen.web.dashboard.client
  - HomeScreen.Web/HomeScreen.Web.Slideshow/homescreen.web.slideshow.client

TODO: Consider a pnpm workspace to install dependencies from the repository root.

3) Provide required parameters (recommended via user secrets or environment)
- For first run, set at least HomeScreenSqlServerPassword, MediaSourceDir, MediaCacheDir.
- If using Azure mapping, set AZURE_MAPS_SUBSCRIPTION_KEY and AZURE_CLIENT_ID.

## Running (local development)

Option A — using Aspire from AppHost (recommended):
- From HomeScreen.AppHost directory:
  - dotnet run
- The AppHost will:
  - Start Redis and SQL Server containers
  - Apply media DB migrations
  - Start services and web servers
- Health checks are exposed at /alive on web servers. Aspire dashboard can be used for observability if enabled.

Option B — run individually (advanced):
- Start infrastructure (Redis/SQL) yourself or with containers
- Run each service with dotnet run from its project directory
- For front-end dev servers (Vite), see scripts below and configure the server to proxy to the ASP.NET servers as needed

## Front-end Scripts

homescreen.web.slideshow.client (Vite + Vue):
- pnpm dev — start Vite dev server
- pnpm build — type-check and build
- pnpm preview — preview built app
- pnpm test:unit — run vitest
- pnpm storybook — run Storybook (port 6006)
- pnpm build-storybook — build Storybook
- pnpm check — run Biome checks

homescreen.web.dashboard.client (Vite + Vue):
- pnpm dev — start Vite dev server
- pnpm build — type-check and build
- pnpm preview — preview built app
- pnpm check — run Biome checks

homescreen.web.common.components (Vite + Vue library):
- pnpm dev — start Vite dev server
- pnpm build — type-check and build
- pnpm build-declare — emit declaration files
- pnpm preview — preview
- pnpm test:unit — run vitest
- pnpm storybook — run Storybook
- pnpm build-storybook — build Storybook
- pnpm check — run Biome checks

homescreen.web.common.components.api:
- This project contains generated API helpers; see its package.json for available scripts (build/compile typically). TODO: Document specific scripts if needed.

## .NET Tests

- Unit tests exist for services (Location, Media, Weather) using xUnit
  - Run all tests: dotnet test HomeScreen.sln
- Integration tests using Aspire.Hosting.Testing in HomeScreen.Integration.Tests
  - Run: dotnet test HomeScreen.Integration.Tests

## Database and Migrations

- SQL Server container is provisioned by Aspire (image mcr.microsoft.com/mssql/server:2025-latest)
- Media database migrations project: HomeScreen.Database.MediaDb.Migrations
- AppHost waits for migrations to complete before starting dependent services

## Configuration and APIs

- Common Server exposes OpenAPI (NSwag) and health checks at /alive
- Services communicate via gRPC clients and HTTP/2
- The Common server proxies/downloads media using an HttpClient configured for HTTP/2

## Troubleshooting

- Ensure Docker is running before starting AppHost
- If containers fail to start, check your HomeScreenSqlServerPassword and that required ports are free
- For front-end dev, ensure Node 22+ and pnpm 10+ per engines field; run pnpm install before dev

## License

TODO: Add license details and a LICENSE file at the repository root.

## Acknowledgements / Notes

- This readme was updated to reflect the current stack (.NET 9 + Aspire + Vue/Vite) and may require updates as the solution evolves.
- Do not commit secrets. Use user-secrets or environment variables for sensitive values.

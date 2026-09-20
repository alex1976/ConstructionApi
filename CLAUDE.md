# ConstructionApi

## Summary

A .NET 9 solution that manages construction price list items and assemblies (bill-of-materials-style groupings of price list items), backed by a shared SQLite database. The repo contains two separate, deployable projects:

- **ConstructionApi** (root) — an ASP.NET Core Web API with CRUD + search endpoints, Swagger UI, and CSV-based seeding.
- **McpServer** ([McpServer/](McpServer/)) — a standalone MCP (Model Context Protocol) server that exposes the same data model as tools for AI agents (search + create/update/delete).

Both projects read/write the same `construction.db` SQLite file but do **not** share code — each has its own `AppDbContext` and model classes that must be kept in sync manually (see Key Points).

## Domain Model

- **PriceList** — a single priced item: `Code`, `Description`, `Category`/`Subcategory`, `UnitOfMeasure`, `Price`, plus cost-breakdown percentages (`SafetyPercentage`, `LabourPercentage`, `MaterialPercentage`, `EquipmentPercentage`) and catalog metadata (`CatalogCode`, `CatalogDescription`, `CatalogAuthor`, `CatalogVersion`).
- **Assembly** — a named, categorized group of price list items (`Name`, `Category`).
- **AssemblyPriceList** — join entity (composite key `AssemblyId` + `PriceListId`) linking an Assembly to a PriceList item with:
  - `Type`: `FixedQuantity` (Value = fixed quantity) or `QuantityFactor` (Value = multiplier of the driver quantity)
  - `Value`: decimal quantity/factor
  - `IsDriver`: marks the item that drives quantity for factor-based items
- **Phase** — a named, categorized construction phase (`Name`, `Description`, `Category`).
- **PhaseAssemblyList** — join entity (composite key `PhaseId` + `AssemblyId`) linking a Phase to an Assembly with a `Quantity` of that assembly to use in the phase.

## Project Layout

| Path | Purpose |
|------|---------|
| [Program.cs](Program.cs) | Web API startup: DI, Swagger, auto-migrate + CSV seeding on boot |
| [Controllers/PriceListsController.cs](Controllers/PriceListsController.cs) | `/api/pricelists` — GET all, GET search, GET by id, POST, PUT (full replace), DELETE |
| [Controllers/AssembliesController.cs](Controllers/AssembliesController.cs) | `/api/assemblies` — same CRUD shape, includes nested `AssemblyPriceListItems` |
| [Controllers/PhasesController.cs](Controllers/PhasesController.cs) | `/api/phases` — same CRUD shape, includes nested `PhaseAssemblyListItems` |
| [Models/](Models/) | `PriceList`, `Assembly`, `AssemblyPriceList`, `Phase`, `PhaseAssemblyList` entity classes |
| [Data/AppDbContext.cs](Data/AppDbContext.cs) | EF Core context; configures the composite keys/relations for `AssemblyPriceList` and `PhaseAssemblyList` |
| [Data/DbInitializer.cs](Data/DbInitializer.cs) | Seeds `PriceLists`/`Assemblies` from CSV on first run only (`if (db.PriceLists.Any()) return;`) |
| [Samples/test_materials.csv](Samples/test_materials.csv), [Samples/test_assemblies.csv](Samples/test_assemblies.csv) | Seed data sources |
| [Migrations/](Migrations/) | EF Core migrations for the shared SQLite schema |
| [McpServer/](McpServer/) | Standalone MCP server project (own csproj, own `Models/`, own `Data/AppDbContext.cs`) — see [McpServer/CLAUDE.md](McpServer/CLAUDE.md) |

## Key Points / Gotchas

- **Two duplicate model/DbContext sets.** [McpServer/Models/](McpServer/Models/) and [McpServer/Data/AppDbContext.cs](McpServer/Data/AppDbContext.cs) are hand-copied duplicates of the root project's, not a shared library reference. Any schema change (new field, new entity, relation change) must be applied in **both** places, and a migration only needs to be added once (from the root project) since both point at the same `construction.db`.
- **PUT endpoints are full replacements**, not partial updates — callers must send every field. The MCP server's write tools ([McpServer/PriceListTools.cs](McpServer/PriceListTools.cs), [McpServer/AssemblyTools.cs](McpServer/AssemblyTools.cs), [McpServer/PhaseTools.cs](McpServer/PhaseTools.cs)), by contrast, support partial updates (only provided fields change).
- **Assembly deletes of a PriceList item are guarded in the MCP server**: `DeletePriceList` refuses to delete an item referenced by assemblies unless `force=true`; the Web API's `DELETE /api/assemblies/{id}` / `/api/pricelists/{id}` have no such guard.
- **Seeding is one-time and idempotent-by-emptiness**: `DbInitializer` only seeds if the respective table is empty, so it won't overwrite existing data on restart.
- **Migrations auto-apply on startup** (`db.Database.Migrate()` in [Program.cs](Program.cs)) — no manual `dotnet ef database update` needed in normal dev flow, but `dotnet ef migrations add <Name>` is still required after entity changes.
- MCP tool names are exposed over the wire in snake_case (e.g. `create_price_list`, `search_assemblies`) even though the C# method names are PascalCase.

## Build & Run

```
dotnet build                          # build the Web API
dotnet run                            # run Web API (Swagger at /swagger)
dotnet run --project McpServer        # run the MCP server (endpoint at /mcp)
```

### Docker

```
docker compose up --build             # build + run both services, sharing construction.db via the db-data volume
```

- [Dockerfile](Dockerfile) / [McpServer/Dockerfile](McpServer/Dockerfile) — multi-stage builds (SDK image → aspnet runtime image) for each project.
- [docker-compose.yml](docker-compose.yml) — runs `api` (port 5000→8080) and `mcp-server` (port 5100→8080), both mounting the `db-data` volume at `/data` and pointing `ConnectionStrings__DefaultConnection` at `/data/construction.db`.
- Only the `api` service runs `db.Database.Migrate()`; compose's `depends_on` only orders container start, not readiness, so the API should finish migrating/seeding before the MCP server hits the same file in practice, but there's no explicit health-gate between them.

## Reference

Full endpoint/tool documentation with request/response examples lives in [README.md](README.md) and [McpServer/README.md](McpServer/README.md).

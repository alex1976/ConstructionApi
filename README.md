# ConstructionApi

A .NET 9 Web API that exposes PriceList and Assemblies data from a SQLite database, seeded from a CSV file.

## Endpoints

### GET /api/pricelists
Returns all price list items.

```
GET /api/pricelists
```

### GET /api/pricelists/search
Filters items by optional query parameters (AND logic, substring match).

```
GET /api/pricelists/search?category=Structural&catalogCode=REG
```

| Parameter | Type | Description |
|-----------|------|-------------|
| catalogCode | string | Filter by catalog code |
| catalogDescription | string | Filter by catalog description |
| catalogAuthor | string | Filter by catalog author |
| code | string | Filter by item code |
| description | string | Filter by description |
| category | string | Filter by category |
| subcategory | string | Filter by subcategory |

### GET /api/pricelists/{id}
Returns a single item by ID.

```
GET /api/pricelists/1
```

### POST /api/pricelists
Creates a new price list item.

```
POST /api/pricelists
Content-Type: application/json

{
  "catalogCode": "REG",
  "catalogDescription": "Prezzario Regionale",
  "catalogAuthor": "Regione",
  "catalogVersion": "2026",
  "code": "ITM9999",
  "description": "Sample item",
  "category": "Structural",
  "subcategory": "Concrete",
  "unitOfMeasure": "m2",
  "price": 100.00,
  "safetyPercentage": 5,
  "labourPercentage": 35,
  "materialPercentage": 50,
  "equipmentPercentage": 10
}
```

### PUT /api/pricelists/{id}
Fully updates an existing item.

```
PUT /api/pricelists/1
Content-Type: application/json

{
  "id": 1,
  "catalogCode": "REG",
  "catalogDescription": "Prezzario Regionale",
  "catalogAuthor": "Regione",
  "catalogVersion": "2026",
  "code": "ITM0001",
  "description": "Updated description",
  "category": "MEP",
  "subcategory": "Electrical",
  "unitOfMeasure": "m2",
  "price": 400.00,
  "safetyPercentage": 10,
  "labourPercentage": 30,
  "materialPercentage": 45,
  "equipmentPercentage": 15
}
```

### DELETE /api/pricelists/{id}
Deletes an item by ID.

```
DELETE /api/pricelists/1
```

## Assembly Endpoints

### GET /api/assemblies
Returns all assemblies with their referenced price list item.

```
GET /api/assemblies
```

### GET /api/assemblies/search
Filters assemblies by optional query parameters (AND logic, substring match).

```
GET /api/assemblies/search?category=Structural
```

| Parameter | Type | Description |
|-----------|------|-------------|
| name | string | Filter by assembly name |
| category | string | Filter by category |

### GET /api/assemblies/{id}
Returns a single assembly by ID.

```
GET /api/assemblies/1
```

### POST /api/assemblies
Creates a new assembly.

```
POST /api/assemblies
Content-Type: application/json

{
  "name": "Foundation Assembly",
  "category": "Structural",
  "assemblyPriceListItems": [
    { "priceListId": 1, "type": "FixedQuantity", "value": 3.5, "isDriver": true },
    { "priceListId": 2, "type": "QuantityFactor", "value": 1.25, "isDriver": false }
  ]
}
```

Each item in `assemblyPriceListItems` requires `priceListId`, `type` (`FixedQuantity` or `QuantityFactor`), `value`, and `isDriver`.

### PUT /api/assemblies/{id}
Fully updates an existing assembly.

```
PUT /api/assemblies/1
Content-Type: application/json

{
  "id": 1,
  "name": "Beam Assembly",
  "category": "Structural",
  "assemblyPriceListItems": [
    { "assemblyId": 1, "priceListId": 3, "type": "QuantityFactor", "value": 1.25, "isDriver": false },
    { "assemblyId": 1, "priceListId": 7, "type": "FixedQuantity", "value": 2.0, "isDriver": true }
  ]
}
```

### DELETE /api/assemblies/{id}
Deletes an assembly by ID.

```
DELETE /api/assemblies/1
```

## Build

```
dotnet build
```

## Run

```
dotnet run
```

The API starts on `http://localhost:{port}`. Swagger UI is available at `/swagger`.

## MCP Server

The `McpServer/` folder contains a standalone MCP (Model Context Protocol) server that exposes search capabilities for AI agents over HTTP.

**Run the MCP server:**

```
dotnet run --project McpServer
```

The MCP server listens on `http://localhost:{port}` and exposes the MCP endpoint at `/mcp`.

**Available tools:**

| Tool | Description |
|------|-------------|
| `SearchPriceLists` | Search price list items by catalog code, catalog description, catalog author, code, description, category, subcategory |
| `SearchAssemblies` | Search assembly items by name and/or category |

Both projects share the same SQLite database (`construction.db`).

## Migrations

The project uses Entity Framework Core migrations for schema versioning. After changing the `PriceList` entity (or adding new entities), create a migration to update the database schema.

**Create a migration:**

```
dotnet ef migrations add DescriptionOfChange
```

**Apply pending migrations to the database:**

```
dotnet ef database update
```

**Remove the last migration (if not yet applied):**

```
dotnet ef migrations remove
```

**List existing migrations:**

```
dotnet ef migrations list
```

**Generate a SQL script for a migration:**

```
dotnet ef migrations script
```

Migrations are applied automatically on application startup via `db.Database.Migrate()` in `Program.cs`, so the database stays in sync after deployment.

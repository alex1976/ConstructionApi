# MCP Server - Configuration for AI Clients

## Claude Desktop

Add to your `claude_desktop_config.json` (Windows: `%APPDATA%\Claude\claude_desktop_config.json`):

```json
{
  "mcpServers": {
    "construction-api": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Users\\aless\\source\\repos\\ConstructionApi\\McpServer",
        "--urls",
        "http://localhost:5100"
      ]
    }
  }
}
```

## VS Code / Cursor

Add to `.vscode/mcp.json` or `.cursor/mcp.json`:

```json
{
  "mcpServers": {
    "construction-api": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Users\\aless\\source\\repos\\ConstructionApi\\McpServer",
        "--urls",
        "http://localhost:5100"
      ]
    }
  }
}
```

## Running manually (HTTP mode)

```
dotnet run --project McpServer --urls http://localhost:5100
```

The MCP endpoint is available at `http://localhost:5100/mcp`.

## Available Tools

| Tool | Description |
|------|-------------|
| `SearchPriceLists` | Search price list items by catalog code, description, author, code, category, subcategory |
| `SearchAssemblies` | Search assembly items by name and/or category |
| `CreatePriceList` | Create a price list item |
| `UpdatePriceList` | Update a price list item (only the provided fields change) |
| `DeletePriceList` | Delete a price list item; needs `force=true` when it is used by assemblies |
| `CreateAssembly` | Create an assembly, optionally with its price list items |
| `UpdateAssembly` | Update an assembly (only the provided fields change; provided items replace all current ones) |
| `DeleteAssembly` | Delete an assembly and its price list links (the price list items are kept) |

Tools are exposed over the wire in snake_case (`search_price_lists`, `create_assembly`, ...).

Assembly items are objects: `{ "priceListId": 1, "type": "FixedQuantity" | "QuantityFactor", "value": 2.5, "isDriver": false }`.

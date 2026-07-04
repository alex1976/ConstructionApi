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

using System.Text.Json;

/// <summary>Small helpers producing the JSON payloads the MCP tools return for non-entity outcomes.</summary>
public static class ToolResult
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static string Ok(string message) =>
        JsonSerializer.Serialize(new { success = true, message }, JsonOptions);

    public static string Error(string message) =>
        JsonSerializer.Serialize(new { success = false, error = message }, JsonOptions);
}

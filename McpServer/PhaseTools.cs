using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using ConstructionApi.McpServer.Data;
using ConstructionApi.McpServer.Models;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

/// <summary>Assembly to attach to a phase.</summary>
public class PhaseAssemblyItemInput
{
    [Description("Id of the assembly")]
    public int AssemblyId { get; set; }

    [Description("Quantity of the assembly to use in the phase")]
    public decimal Quantity { get; set; }
}

[McpServerToolType]
public class PhaseTools
{
    private static readonly JsonSerializerOptions JsonOptions =
        new() { WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles };

    [McpServerTool, Description("Create a new phase, optionally with its assemblies")]
    public static async Task<string> CreatePhase(
        AppDbContext db,
        [Description("Phase name")] string name,
        [Description("Phase description")] string? description = null,
        [Description("Category")] string? category = null,
        [Description("Assemblies composing the phase")] PhaseAssemblyItemInput[]? items = null)
    {
        var (phaseItems, error) = await BuildItemsAsync(db, items);
        if (error is not null) return ToolResult.Error(error);

        var phase = new Phase
        {
            Name = name,
            Description = description ?? string.Empty,
            Category = category ?? string.Empty,
            PhaseAssemblyListItems = phaseItems
        };

        db.Phases.Add(phase);
        await db.SaveChangesAsync();

        return await SerializeAsync(db, phase.Id);
    }

    [McpServerTool, Description("Update an existing phase. Only the fields that are provided are changed; when items are provided they replace all the current ones")]
    public static async Task<string> UpdatePhase(
        AppDbContext db,
        [Description("Id of the phase to update")] int id,
        [Description("New phase name")] string? name = null,
        [Description("New description")] string? description = null,
        [Description("New category")] string? category = null,
        [Description("Assemblies replacing all the current ones")] PhaseAssemblyItemInput[]? items = null)
    {
        var phase = await db.Phases
            .Include(p => p.PhaseAssemblyListItems)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (phase is null)
            return ToolResult.Error($"Phase {id} not found");

        var (phaseItems, error) = await BuildItemsAsync(db, items);
        if (error is not null) return ToolResult.Error(error);

        if (name is not null) phase.Name = name;
        if (description is not null) phase.Description = description;
        if (category is not null) phase.Category = category;

        if (items is not null)
        {
            db.PhaseAssemblyLists.RemoveRange(phase.PhaseAssemblyListItems);
            phase.PhaseAssemblyListItems = phaseItems;
        }

        await db.SaveChangesAsync();

        return await SerializeAsync(db, phase.Id);
    }

    [McpServerTool, Description("Delete a phase and its assembly links. The assemblies themselves are not deleted")]
    public static async Task<string> DeletePhase(
        AppDbContext db,
        [Description("Id of the phase to delete")] int id)
    {
        var phase = await db.Phases.FindAsync(id);
        if (phase is null)
            return ToolResult.Error($"Phase {id} not found");

        db.Phases.Remove(phase);
        await db.SaveChangesAsync();

        return ToolResult.Ok($"Phase {id} deleted");
    }

    private static async Task<(List<PhaseAssemblyList> Items, string? Error)> BuildItemsAsync(
        AppDbContext db, PhaseAssemblyItemInput[]? items)
    {
        if (items is null || items.Length == 0)
            return ([], null);

        var duplicates = items.GroupBy(i => i.AssemblyId).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicates.Count > 0)
            return ([], $"Assemblies listed more than once: {string.Join(", ", duplicates)}");

        var ids = items.Select(i => i.AssemblyId).ToList();
        var existingIds = await db.Assemblies.Where(a => ids.Contains(a.Id)).Select(a => a.Id).ToListAsync();
        var missing = ids.Except(existingIds).ToList();
        if (missing.Count > 0)
            return ([], $"Assemblies not found: {string.Join(", ", missing)}");

        var result = items.Select(item => new PhaseAssemblyList
        {
            AssemblyId = item.AssemblyId,
            Quantity = item.Quantity
        }).ToList();

        return (result, null);
    }

    private static async Task<string> SerializeAsync(AppDbContext db, int id)
    {
        var phase = await db.Phases
            .Include(p => p.PhaseAssemblyListItems)
            .ThenInclude(pa => pa.Assembly)
            .FirstAsync(p => p.Id == id);

        return JsonSerializer.Serialize(phase, JsonOptions);
    }
}

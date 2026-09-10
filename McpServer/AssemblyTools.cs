using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using ConstructionApi.McpServer.Data;
using ConstructionApi.McpServer.Models;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

/// <summary>Price list item to attach to an assembly.</summary>
public class AssemblyItemInput
{
    [Description("Id of the price list item")]
    public int PriceListId { get; set; }

    [Description("FixedQuantity (Value is the quantity) or QuantityFactor (Value multiplies the driver quantity)")]
    public string Type { get; set; } = nameof(AssemblyItemType.FixedQuantity);

    [Description("Quantity or factor, depending on Type")]
    public decimal Value { get; set; }

    [Description("True if this item drives the quantity of the other items")]
    public bool IsDriver { get; set; }
}

[McpServerToolType]
public class AssemblyTools
{
    private static readonly JsonSerializerOptions JsonOptions =
        new() { WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles };

    [McpServerTool, Description("Create a new assembly, optionally with its price list items")]
    public static async Task<string> CreateAssembly(
        AppDbContext db,
        [Description("Assembly name")] string name,
        [Description("Category")] string? category = null,
        [Description("Price list items composing the assembly")] AssemblyItemInput[]? items = null)
    {
        var (assemblyItems, error) = await BuildItemsAsync(db, items);
        if (error is not null) return ToolResult.Error(error);

        var assembly = new Assembly
        {
            Name = name,
            Category = category ?? string.Empty,
            AssemblyPriceListItems = assemblyItems
        };

        db.Assemblies.Add(assembly);
        await db.SaveChangesAsync();

        return await SerializeAsync(db, assembly.Id);
    }

    [McpServerTool, Description("Update an existing assembly. Only the fields that are provided are changed; when items are provided they replace all the current ones")]
    public static async Task<string> UpdateAssembly(
        AppDbContext db,
        [Description("Id of the assembly to update")] int id,
        [Description("New assembly name")] string? name = null,
        [Description("New category")] string? category = null,
        [Description("Price list items replacing all the current ones")] AssemblyItemInput[]? items = null)
    {
        var assembly = await db.Assemblies
            .Include(a => a.AssemblyPriceListItems)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (assembly is null)
            return ToolResult.Error($"Assembly {id} not found");

        var (assemblyItems, error) = await BuildItemsAsync(db, items);
        if (error is not null) return ToolResult.Error(error);

        if (name is not null) assembly.Name = name;
        if (category is not null) assembly.Category = category;

        if (items is not null)
        {
            db.AssemblyPriceLists.RemoveRange(assembly.AssemblyPriceListItems);
            assembly.AssemblyPriceListItems = assemblyItems;
        }

        await db.SaveChangesAsync();

        return await SerializeAsync(db, assembly.Id);
    }

    [McpServerTool, Description("Delete an assembly and its price list item links. The price list items themselves are not deleted")]
    public static async Task<string> DeleteAssembly(
        AppDbContext db,
        [Description("Id of the assembly to delete")] int id)
    {
        var assembly = await db.Assemblies.FindAsync(id);
        if (assembly is null)
            return ToolResult.Error($"Assembly {id} not found");

        db.Assemblies.Remove(assembly);
        await db.SaveChangesAsync();

        return ToolResult.Ok($"Assembly {id} deleted");
    }

    private static async Task<(List<AssemblyPriceList> Items, string? Error)> BuildItemsAsync(
        AppDbContext db, AssemblyItemInput[]? items)
    {
        if (items is null || items.Length == 0)
            return ([], null);

        var duplicates = items.GroupBy(i => i.PriceListId).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicates.Count > 0)
            return ([], $"Price list items listed more than once: {string.Join(", ", duplicates)}");

        var ids = items.Select(i => i.PriceListId).ToList();
        var existingIds = await db.PriceLists.Where(p => ids.Contains(p.Id)).Select(p => p.Id).ToListAsync();
        var missing = ids.Except(existingIds).ToList();
        if (missing.Count > 0)
            return ([], $"Price list items not found: {string.Join(", ", missing)}");

        var result = new List<AssemblyPriceList>();
        foreach (var item in items)
        {
            if (!Enum.TryParse<AssemblyItemType>(item.Type, ignoreCase: true, out var type))
                return ([], $"Invalid type '{item.Type}' for price list item {item.PriceListId}. " +
                            $"Valid values: {string.Join(", ", Enum.GetNames<AssemblyItemType>())}");

            result.Add(new AssemblyPriceList
            {
                PriceListId = item.PriceListId,
                Type = type,
                Value = item.Value,
                IsDriver = item.IsDriver
            });
        }

        return (result, null);
    }

    private static async Task<string> SerializeAsync(AppDbContext db, int id)
    {
        var assembly = await db.Assemblies
            .Include(a => a.AssemblyPriceListItems)
            .ThenInclude(ap => ap.PriceList)
            .FirstAsync(a => a.Id == id);

        return JsonSerializer.Serialize(assembly, JsonOptions);
    }
}

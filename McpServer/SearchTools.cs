using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using ConstructionApi.McpServer.Data;
using ConstructionApi.McpServer.Models;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

[McpServerToolType]
public class SearchTools
{
    [McpServerTool, Description("Search price list items by code, description, category, subcategory, catalog code, catalog description, or catalog author")]
    public static async Task<string> SearchPriceLists(
        AppDbContext db,
        [Description("Filter by catalog code (fuzzy, tolerates typos)")] string? catalogCode = null,
        [Description("Filter by catalog description (fuzzy, tolerates typos)")] string? catalogDescription = null,
        [Description("Filter by catalog author (fuzzy, tolerates typos)")] string? catalogAuthor = null,
        [Description("Filter by item code (fuzzy, tolerates typos)")] string? code = null,
        [Description("Filter by description (fuzzy, tolerates typos)")] string? description = null,
        [Description("Filter by category (fuzzy, tolerates typos)")] string? category = null,
        [Description("Filter by subcategory (fuzzy, tolerates typos)")] string? subcategory = null)
    {
        var items = await db.PriceLists.ToListAsync();

        var results = items.Where(p =>
            FuzzySearch.IsMatch(p.CatalogCode, catalogCode) &&
            FuzzySearch.IsMatch(p.CatalogDescription, catalogDescription) &&
            FuzzySearch.IsMatch(p.CatalogAuthor, catalogAuthor) &&
            FuzzySearch.IsMatch(p.Code, code) &&
            FuzzySearch.IsMatch(p.Description, description) &&
            FuzzySearch.IsMatch(p.Category, category) &&
            FuzzySearch.IsMatch(p.Subcategory, subcategory))
            .ToList();

        return JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Search assembly items by name and/or category")]
    public static async Task<string> SearchAssemblies(
        AppDbContext db,
        [Description("Filter by assembly name (fuzzy, tolerates typos)")] string? name = null,
        [Description("Filter by category (fuzzy, tolerates typos)")] string? category = null)
    {
        var items = await db.Assemblies.Include(a => a.AssemblyPriceListItems).ThenInclude(ap => ap.PriceList).ToListAsync();

        var results = items.Where(a =>
            FuzzySearch.IsMatch(a.Name, name) &&
            FuzzySearch.IsMatch(a.Category, category))
            .ToList();

        return JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }

    [McpServerTool, Description("Search phases by name, description, and/or category")]
    public static async Task<string> SearchPhases(
        AppDbContext db,
        [Description("Filter by phase name (fuzzy, tolerates typos)")] string? name = null,
        [Description("Filter by description (fuzzy, tolerates typos)")] string? description = null,
        [Description("Filter by category (fuzzy, tolerates typos)")] string? category = null)
    {
        var items = await db.Phases.Include(p => p.PhaseAssemblyListItems).ThenInclude(pa => pa.Assembly).ToListAsync();

        var results = items.Where(p =>
            FuzzySearch.IsMatch(p.Name, name) &&
            FuzzySearch.IsMatch(p.Description, description) &&
            FuzzySearch.IsMatch(p.Category, category))
            .ToList();

        return JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }
}

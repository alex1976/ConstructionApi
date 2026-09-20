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
        [Description("Filter by catalog code (substring)")] string? catalogCode = null,
        [Description("Filter by catalog description (substring)")] string? catalogDescription = null,
        [Description("Filter by catalog author (substring)")] string? catalogAuthor = null,
        [Description("Filter by item code (substring)")] string? code = null,
        [Description("Filter by description (substring)")] string? description = null,
        [Description("Filter by category (substring)")] string? category = null,
        [Description("Filter by subcategory (substring)")] string? subcategory = null)
    {
        var query = db.PriceLists.AsQueryable();

        if (!string.IsNullOrWhiteSpace(catalogCode))
            query = query.Where(p => p.CatalogCode.Contains(catalogCode));
        if (!string.IsNullOrWhiteSpace(catalogDescription))
            query = query.Where(p => p.CatalogDescription.Contains(catalogDescription));
        if (!string.IsNullOrWhiteSpace(catalogAuthor))
            query = query.Where(p => p.CatalogAuthor.Contains(catalogAuthor));
        if (!string.IsNullOrWhiteSpace(code))
            query = query.Where(p => p.Code.Contains(code));
        if (!string.IsNullOrWhiteSpace(description))
            query = query.Where(p => p.Description.Contains(description));
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category.Contains(category));
        if (!string.IsNullOrWhiteSpace(subcategory))
            query = query.Where(p => p.Subcategory.Contains(subcategory));

        var results = await query.ToListAsync();
        return JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Search assembly items by name and/or category")]
    public static async Task<string> SearchAssemblies(
        AppDbContext db,
        [Description("Filter by assembly name (substring)")] string? name = null,
        [Description("Filter by category (substring)")] string? category = null)
    {
        var query = db.Assemblies.Include(a => a.AssemblyPriceListItems).ThenInclude(ap => ap.PriceList).AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(a => a.Name.Contains(name));
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(a => a.Category.Contains(category));

        var results = await query.ToListAsync();
        return JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }

    [McpServerTool, Description("Search phases by name, description, and/or category")]
    public static async Task<string> SearchPhases(
        AppDbContext db,
        [Description("Filter by phase name (substring)")] string? name = null,
        [Description("Filter by description (substring)")] string? description = null,
        [Description("Filter by category (substring)")] string? category = null)
    {
        var query = db.Phases.Include(p => p.PhaseAssemblyListItems).ThenInclude(pa => pa.Assembly).AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name));
        if (!string.IsNullOrWhiteSpace(description))
            query = query.Where(p => p.Description.Contains(description));
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category.Contains(category));

        var results = await query.ToListAsync();
        return JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }
}

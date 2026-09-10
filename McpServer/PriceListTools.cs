using System.ComponentModel;
using System.Text.Json;
using ConstructionApi.McpServer.Data;
using ConstructionApi.McpServer.Models;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

[McpServerToolType]
public class PriceListTools
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [McpServerTool, Description("Create a new price list item")]
    public static async Task<string> CreatePriceList(
        AppDbContext db,
        [Description("Item code")] string code,
        [Description("Item description")] string description,
        [Description("Catalog code")] string? catalogCode = null,
        [Description("Catalog description")] string? catalogDescription = null,
        [Description("Catalog author")] string? catalogAuthor = null,
        [Description("Catalog version")] string? catalogVersion = null,
        [Description("Category")] string? category = null,
        [Description("Subcategory")] string? subcategory = null,
        [Description("Unit of measure")] string? unitOfMeasure = null,
        [Description("Unit price")] decimal price = 0,
        [Description("Safety cost percentage")] decimal safetyPercentage = 0,
        [Description("Labour cost percentage")] decimal labourPercentage = 0,
        [Description("Material cost percentage")] decimal materialPercentage = 0,
        [Description("Equipment cost percentage")] decimal equipmentPercentage = 0)
    {
        var item = new PriceList
        {
            CatalogCode = catalogCode ?? string.Empty,
            CatalogDescription = catalogDescription ?? string.Empty,
            CatalogAuthor = catalogAuthor ?? string.Empty,
            CatalogVersion = catalogVersion ?? string.Empty,
            Code = code,
            Description = description,
            Category = category ?? string.Empty,
            Subcategory = subcategory ?? string.Empty,
            UnitOfMeasure = unitOfMeasure ?? string.Empty,
            Price = price,
            SafetyPercentage = safetyPercentage,
            LabourPercentage = labourPercentage,
            MaterialPercentage = materialPercentage,
            EquipmentPercentage = equipmentPercentage
        };

        db.PriceLists.Add(item);
        await db.SaveChangesAsync();

        return JsonSerializer.Serialize(item, JsonOptions);
    }

    [McpServerTool, Description("Update an existing price list item. Only the fields that are provided are changed")]
    public static async Task<string> UpdatePriceList(
        AppDbContext db,
        [Description("Id of the price list item to update")] int id,
        [Description("New item code")] string? code = null,
        [Description("New item description")] string? description = null,
        [Description("New catalog code")] string? catalogCode = null,
        [Description("New catalog description")] string? catalogDescription = null,
        [Description("New catalog author")] string? catalogAuthor = null,
        [Description("New catalog version")] string? catalogVersion = null,
        [Description("New category")] string? category = null,
        [Description("New subcategory")] string? subcategory = null,
        [Description("New unit of measure")] string? unitOfMeasure = null,
        [Description("New unit price")] decimal? price = null,
        [Description("New safety cost percentage")] decimal? safetyPercentage = null,
        [Description("New labour cost percentage")] decimal? labourPercentage = null,
        [Description("New material cost percentage")] decimal? materialPercentage = null,
        [Description("New equipment cost percentage")] decimal? equipmentPercentage = null)
    {
        var item = await db.PriceLists.FindAsync(id);
        if (item is null)
            return ToolResult.Error($"Price list item {id} not found");

        if (code is not null) item.Code = code;
        if (description is not null) item.Description = description;
        if (catalogCode is not null) item.CatalogCode = catalogCode;
        if (catalogDescription is not null) item.CatalogDescription = catalogDescription;
        if (catalogAuthor is not null) item.CatalogAuthor = catalogAuthor;
        if (catalogVersion is not null) item.CatalogVersion = catalogVersion;
        if (category is not null) item.Category = category;
        if (subcategory is not null) item.Subcategory = subcategory;
        if (unitOfMeasure is not null) item.UnitOfMeasure = unitOfMeasure;
        if (price is not null) item.Price = price.Value;
        if (safetyPercentage is not null) item.SafetyPercentage = safetyPercentage.Value;
        if (labourPercentage is not null) item.LabourPercentage = labourPercentage.Value;
        if (materialPercentage is not null) item.MaterialPercentage = materialPercentage.Value;
        if (equipmentPercentage is not null) item.EquipmentPercentage = equipmentPercentage.Value;

        await db.SaveChangesAsync();

        return JsonSerializer.Serialize(item, JsonOptions);
    }

    [McpServerTool, Description("Delete a price list item. Fails if the item is used by assemblies unless force is true, in which case it is also removed from those assemblies")]
    public static async Task<string> DeletePriceList(
        AppDbContext db,
        [Description("Id of the price list item to delete")] int id,
        [Description("Delete the item even if it is used by assemblies")] bool force = false)
    {
        var item = await db.PriceLists.FindAsync(id);
        if (item is null)
            return ToolResult.Error($"Price list item {id} not found");

        var usedBy = await db.AssemblyPriceLists
            .Where(ap => ap.PriceListId == id)
            .Select(ap => new { ap.AssemblyId, ap.Assembly.Name })
            .ToListAsync();

        if (usedBy.Count > 0 && !force)
            return ToolResult.Error(
                $"Price list item {id} is used by {usedBy.Count} assembly/assemblies: " +
                string.Join(", ", usedBy.Select(u => $"{u.AssemblyId} ({u.Name})")) +
                ". Call again with force=true to delete it and remove it from those assemblies.");

        db.PriceLists.Remove(item);
        await db.SaveChangesAsync();

        return ToolResult.Ok($"Price list item {id} deleted" +
            (usedBy.Count > 0 ? $", removed from {usedBy.Count} assembly/assemblies" : string.Empty));
    }
}

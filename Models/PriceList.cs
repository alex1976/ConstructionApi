namespace ConstructionApi.Models;

public class PriceList
{
    public int Id { get; set; }
    public string CatalogCode { get; set; } = string.Empty;
    public string CatalogDescription { get; set; } = string.Empty;
    public string CatalogAuthor { get; set; } = string.Empty;
    public string CatalogVersion { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Subcategory { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal SafetyPercentage { get; set; }
    public decimal LabourPercentage { get; set; }
    public decimal MaterialPercentage { get; set; }
    public decimal EquipmentPercentage { get; set; }
}

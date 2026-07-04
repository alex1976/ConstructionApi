namespace ConstructionApi.McpServer.Models;

public enum AssemblyType
{
    FixedQuantity,
    QuantityFactor
}

public class Assembly
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public AssemblyType Type { get; set; }
    public decimal Value { get; set; }
    public bool IsDriver { get; set; }

    public int PriceListId { get; set; }
    public PriceList PriceList { get; set; } = null!;
}

namespace ConstructionApi.Models;

public enum AssemblyItemType
{
    FixedQuantity,
    QuantityFactor
}

public class AssemblyPriceList
{
    public int AssemblyId { get; set; }
    public Assembly Assembly { get; set; } = null!;
    public int PriceListId { get; set; }
    public PriceList PriceList { get; set; } = null!;
    public AssemblyItemType Type { get; set; }
    public decimal Value { get; set; }
    public bool IsDriver { get; set; }
}

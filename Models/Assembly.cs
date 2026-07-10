namespace ConstructionApi.Models;

public class Assembly
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public ICollection<AssemblyPriceList> AssemblyPriceListItems { get; set; } = [];
}

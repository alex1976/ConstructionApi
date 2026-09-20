namespace ConstructionApi.McpServer.Models;

public class Phase
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public ICollection<PhaseAssemblyList> PhaseAssemblyListItems { get; set; } = [];
}

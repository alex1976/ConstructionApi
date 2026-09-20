namespace ConstructionApi.McpServer.Models;

public class PhaseAssemblyList
{
    public int PhaseId { get; set; }
    public Phase Phase { get; set; } = null!;
    public int AssemblyId { get; set; }
    public Assembly Assembly { get; set; } = null!;
    public decimal Quantity { get; set; }
}

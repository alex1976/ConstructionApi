using Microsoft.EntityFrameworkCore;
using ConstructionApi.McpServer.Models;

namespace ConstructionApi.McpServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<Assembly> Assemblies => Set<Assembly>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assembly>()
            .HasOne(a => a.PriceList)
            .WithMany()
            .HasForeignKey(a => a.PriceListId);
    }
}

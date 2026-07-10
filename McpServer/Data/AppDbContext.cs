using Microsoft.EntityFrameworkCore;
using ConstructionApi.McpServer.Models;

namespace ConstructionApi.McpServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<Assembly> Assemblies => Set<Assembly>();
    public DbSet<AssemblyPriceList> AssemblyPriceLists => Set<AssemblyPriceList>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssemblyPriceList>(entity =>
        {
            entity.HasKey(e => new { e.AssemblyId, e.PriceListId });
            entity.HasOne(e => e.Assembly)
                .WithMany(a => a.AssemblyPriceListItems)
                .HasForeignKey(e => e.AssemblyId);
            entity.HasOne(e => e.PriceList)
                .WithMany()
                .HasForeignKey(e => e.PriceListId);
        });
    }
}

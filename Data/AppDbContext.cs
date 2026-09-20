using Microsoft.EntityFrameworkCore;
using ConstructionApi.Models;

namespace ConstructionApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<Assembly> Assemblies => Set<Assembly>();
    public DbSet<AssemblyPriceList> AssemblyPriceLists => Set<AssemblyPriceList>();
    public DbSet<Phase> Phases => Set<Phase>();
    public DbSet<PhaseAssemblyList> PhaseAssemblyLists => Set<PhaseAssemblyList>();

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

        modelBuilder.Entity<PhaseAssemblyList>(entity =>
        {
            entity.HasKey(e => new { e.PhaseId, e.AssemblyId });
            entity.HasOne(e => e.Phase)
                .WithMany(p => p.PhaseAssemblyListItems)
                .HasForeignKey(e => e.PhaseId);
            entity.HasOne(e => e.Assembly)
                .WithMany()
                .HasForeignKey(e => e.AssemblyId);
        });
    }
}

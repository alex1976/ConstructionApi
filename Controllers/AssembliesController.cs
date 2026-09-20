using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConstructionApi.Data;
using ConstructionApi.Models;

namespace ConstructionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssembliesController : ControllerBase
{
    private readonly AppDbContext _db;

    public AssembliesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Assembly>>> GetAll()
    {
        return await _db.Assemblies
            .Include(a => a.AssemblyPriceListItems)
            .ThenInclude(ap => ap.PriceList)
            .ToListAsync();
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<Assembly>>> Search(
        [FromQuery] string? name = null,
        [FromQuery] string? category = null)
    {
        var items = await _db.Assemblies
            .Include(a => a.AssemblyPriceListItems)
            .ThenInclude(ap => ap.PriceList)
            .ToListAsync();

        return items.Where(a =>
            FuzzySearch.IsMatch(a.Name, name) &&
            FuzzySearch.IsMatch(a.Category, category))
            .ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Assembly>> GetById(int id)
    {
        var item = await _db.Assemblies
            .Include(a => a.AssemblyPriceListItems)
            .ThenInclude(ap => ap.PriceList)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (item is null) return NotFound();
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<Assembly>> Create(Assembly assembly)
    {
        _db.Assemblies.Add(assembly);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = assembly.Id }, assembly);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Assembly assembly)
    {
        if (id != assembly.Id) return BadRequest();

        var existing = await _db.Assemblies
            .Include(a => a.AssemblyPriceListItems)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (existing is null) return NotFound();

        existing.Name = assembly.Name;
        existing.Category = assembly.Category;

        _db.AssemblyPriceLists.RemoveRange(existing.AssemblyPriceListItems);
        existing.AssemblyPriceListItems = assembly.AssemblyPriceListItems;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var item = await _db.Assemblies.FindAsync(id);
        if (item is null) return NotFound();

        _db.Assemblies.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

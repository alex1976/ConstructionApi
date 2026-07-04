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
        return await _db.Assemblies.Include(a => a.PriceList).ToListAsync();
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<Assembly>>> Search(
        [FromQuery] string? name = null,
        [FromQuery] string? category = null)
    {
        var query = _db.Assemblies.Include(a => a.PriceList).AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(a => a.Name.Contains(name));
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(a => a.Category.Contains(category));

        return await query.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Assembly>> GetById(int id)
    {
        var item = await _db.Assemblies.Include(a => a.PriceList).FirstOrDefaultAsync(a => a.Id == id);
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

        var existing = await _db.Assemblies.FindAsync(id);
        if (existing is null) return NotFound();

        existing.Name = assembly.Name;
        existing.Category = assembly.Category;
        existing.Type = assembly.Type;
        existing.Value = assembly.Value;
        existing.IsDriver = assembly.IsDriver;
        existing.PriceListId = assembly.PriceListId;

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

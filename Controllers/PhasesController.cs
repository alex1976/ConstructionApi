using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConstructionApi.Data;
using ConstructionApi.Models;

namespace ConstructionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhasesController : ControllerBase
{
    private readonly AppDbContext _db;

    public PhasesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Phase>>> GetAll()
    {
        return await _db.Phases
            .Include(p => p.PhaseAssemblyListItems)
            .ThenInclude(pa => pa.Assembly)
            .ToListAsync();
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<Phase>>> Search(
        [FromQuery] string? name = null,
        [FromQuery] string? category = null)
    {
        var items = await _db.Phases
            .Include(p => p.PhaseAssemblyListItems)
            .ThenInclude(pa => pa.Assembly)
            .ToListAsync();

        return items.Where(p =>
            FuzzySearch.IsMatch(p.Name, name) &&
            FuzzySearch.IsMatch(p.Category, category))
            .ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Phase>> GetById(int id)
    {
        var item = await _db.Phases
            .Include(p => p.PhaseAssemblyListItems)
            .ThenInclude(pa => pa.Assembly)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (item is null) return NotFound();
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<Phase>> Create(Phase phase)
    {
        _db.Phases.Add(phase);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = phase.Id }, phase);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Phase phase)
    {
        if (id != phase.Id) return BadRequest();

        var existing = await _db.Phases
            .Include(p => p.PhaseAssemblyListItems)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (existing is null) return NotFound();

        existing.Name = phase.Name;
        existing.Description = phase.Description;
        existing.Category = phase.Category;

        _db.PhaseAssemblyLists.RemoveRange(existing.PhaseAssemblyListItems);
        existing.PhaseAssemblyListItems = phase.PhaseAssemblyListItems;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var item = await _db.Phases.FindAsync(id);
        if (item is null) return NotFound();

        _db.Phases.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

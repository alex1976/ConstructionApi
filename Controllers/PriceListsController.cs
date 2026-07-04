using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConstructionApi.Data;
using ConstructionApi.Models;

namespace ConstructionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriceListsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PriceListsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<PriceList>>> GetAll()
    {
        return await _db.PriceLists.ToListAsync();
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<PriceList>>> Search(
        [FromQuery] string? catalogCode = null,
        [FromQuery] string? catalogDescription = null,
        [FromQuery] string? catalogAuthor = null,
        [FromQuery] string? code = null,
        [FromQuery] string? description = null,
        [FromQuery] string? category = null,
        [FromQuery] string? subcategory = null)
    {
        var query = _db.PriceLists.AsQueryable();

        if (!string.IsNullOrWhiteSpace(catalogCode))
            query = query.Where(p => p.CatalogCode.Contains(catalogCode));
        if (!string.IsNullOrWhiteSpace(catalogDescription))
            query = query.Where(p => p.CatalogDescription.Contains(catalogDescription));
        if (!string.IsNullOrWhiteSpace(catalogAuthor))
            query = query.Where(p => p.CatalogAuthor.Contains(catalogAuthor));
        if (!string.IsNullOrWhiteSpace(code))
            query = query.Where(p => p.Code.Contains(code));
        if (!string.IsNullOrWhiteSpace(description))
            query = query.Where(p => p.Description.Contains(description));
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category.Contains(category));
        if (!string.IsNullOrWhiteSpace(subcategory))
            query = query.Where(p => p.Subcategory.Contains(subcategory));

        return await query.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PriceList>> GetById(int id)
    {
        var item = await _db.PriceLists.FindAsync(id);
        if (item is null) return NotFound();
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<PriceList>> Create(PriceList priceList)
    {
        _db.PriceLists.Add(priceList);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = priceList.Id }, priceList);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, PriceList priceList)
    {
        if (id != priceList.Id) return BadRequest();

        var existing = await _db.PriceLists.FindAsync(id);
        if (existing is null) return NotFound();

        existing.CatalogCode = priceList.CatalogCode;
        existing.CatalogDescription = priceList.CatalogDescription;
        existing.CatalogAuthor = priceList.CatalogAuthor;
        existing.CatalogVersion = priceList.CatalogVersion;
        existing.Code = priceList.Code;
        existing.Description = priceList.Description;
        existing.Category = priceList.Category;
        existing.Subcategory = priceList.Subcategory;
        existing.UnitOfMeasure = priceList.UnitOfMeasure;
        existing.Price = priceList.Price;
        existing.SafetyPercentage = priceList.SafetyPercentage;
        existing.LabourPercentage = priceList.LabourPercentage;
        existing.MaterialPercentage = priceList.MaterialPercentage;
        existing.EquipmentPercentage = priceList.EquipmentPercentage;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var item = await _db.PriceLists.FindAsync(id);
        if (item is null) return NotFound();

        _db.PriceLists.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

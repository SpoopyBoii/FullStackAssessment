using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FullStackAssessment.Api.Models;
using FullStackAssessment.Api.DTOs;

namespace FullStackAssessment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductTypesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductTypesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/producttypes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductTypeDto>>> GetProductTypes()
    {
        return await _context.ProductTypes
            .Select(pt => new ProductTypeDto
            {
                Id = pt.Id,
                Name = pt.Name,
                Description = pt.Description
            })
            .ToListAsync();
    }

    // GET: api/producttypes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductTypeDto>> GetProductType(int id)
    {
        var productType = await _context.ProductTypes.FindAsync(id);

        if (productType == null)
        {
            return NotFound();
        }

        return new ProductTypeDto
        {
            Id = productType.Id,
            Name = productType.Name,
            Description = productType.Description
        };
    }

    // POST: api/producttypes
    [HttpPost]
    public async Task<ActionResult<ProductTypeDto>> PostProductType(ProductTypeDto dto)
    {
        if (await _context.ProductTypes.AnyAsync(pt => pt.Name == dto.Name))
        {
            return BadRequest("A product type with this name already exists.");
        }

        var productType = new ProductType
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _context.ProductTypes.Add(productType);
        await _context.SaveChangesAsync();

        dto.Id = productType.Id;
        return CreatedAtAction(nameof(GetProductType), new { id = productType.Id }, dto);
    }

    // PUT: api/producttypes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProductType(int id, ProductTypeDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch.");

        var productType = await _context.ProductTypes.FindAsync(id);
        if (productType == null) return NotFound();

        // Check if name is being changed to an existing one
        if (productType.Name != dto.Name && await _context.ProductTypes.AnyAsync(pt => pt.Name == dto.Name))
        {
            return BadRequest("A product type with this name already exists.");
        }

        productType.Name = dto.Name;
        productType.Description = dto.Description;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductTypeExists(id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/producttypes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductType(int id)
    {
        var productType = await _context.ProductTypes.FindAsync(id);
        if (productType == null) return NotFound();

        // Check for integrity blockages before attempting deletion
        if (await _context.Products.AnyAsync(p => p.ProductTypeId == id))
        {
            return BadRequest("Cannot delete this type because it is assigned to existing products.");
        }

        _context.ProductTypes.Remove(productType);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductTypeExists(int id)
    {
        return _context.ProductTypes.Any(e => e.Id == id);
    }
}
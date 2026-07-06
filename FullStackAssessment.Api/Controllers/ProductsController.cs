using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FullStackAssessment.Api.Models;
using FullStackAssessment.Api.DTOs;

namespace FullStackAssessment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        return await _context.Products
            .Include(p => p.ProductType) // Eager loading ensures we get the descriptive name
            .Select(p => new ProductDto
            {
                Id = p.Id,
                ProductTypeId = p.ProductTypeId,
                ProductTypeName = p.ProductType.Name,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description
            })
            .ToListAsync();
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.ProductType)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        return new ProductDto
        {
            Id = product.Id,
            ProductTypeId = product.ProductTypeId,
            ProductTypeName = product.ProductType.Name,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description
        };
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> PostProduct(ProductCreateUpdateDto dto)
    {
        // business rule: verify category safety before saving
        if (!await _context.ProductTypes.AnyAsync(pt => pt.Id == dto.ProductTypeId))
        {
            return BadRequest("Invalid ProductTypeId. The specified type does not exist.");
        }

        var product = new Product
        {
            ProductTypeId = dto.ProductTypeId,
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return await GetProduct(product.Id);
    }

    // PUT: api/products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, ProductCreateUpdateDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        if (!await _context.ProductTypes.AnyAsync(pt => pt.Id == dto.ProductTypeId))
        {
            return BadRequest("Invalid ProductTypeId. The specified type does not exist.");
        }

        product.ProductTypeId = dto.ProductTypeId;
        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Description = dto.Description;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}
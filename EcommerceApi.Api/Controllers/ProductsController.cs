using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceApi.Api.Data;
using EcommerceApi.Api.Models;
using EcommerceApi.Api.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceApi.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        // 1. Use .Include() to pull in the Category data from the other table
        var products = await _context.Products
            .Include(p => p.Category) 
            .ToListAsync();

        // Map the list of Models to a list of DTOs
        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            // Access the included Category object to get the Name
            CategoryName = p.Category?.Name ?? "No Category" 
        });

        return Ok(productDtos);
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        // Map Model to DTO
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl
        };
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> PostProduct(ProductCreateDto productCreateDto)
    {
        // 1. Map CreateDto -> Model (Preparing for Database)
        var product = new Product
        {
            Name = productCreateDto.Name,
            Description = productCreateDto.Description,
            Price = productCreateDto.Price,
            StockQuantity = productCreateDto.StockQuantity
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // 2. Map Model -> ProductDto (Preparing for User Response)
        var responseDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl
        };

        return CreatedAtAction(nameof(GetProduct), new { id = responseDto.Id }, responseDto);
    }

    // PUT: api/products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, ProductCreateDto updateDto)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        // Update the existing model with values from the DTO
        product.Name = updateDto.Name;
        product.Description = updateDto.Description;
        product.Price = updateDto.Price;
        product.StockQuantity = updateDto.StockQuantity;

        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Products.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/upload-image")]
    public async Task<IActionResult> UploadImage(int id, [FromForm] UploadImageDto input)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound("Product not found");

        if (input.File == null || input.File.Length == 0)
            return BadRequest("No file uploaded");

        // 1. Validate File Extension
        var permittedExtensions = new[] { ".jpg", ".png", ".jpeg" };
        var extension = Path.GetExtension(input.File.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
            return BadRequest("Invalid file type. Only JPG, JPEG, and PNG are allowed.");

        // 2. Create a Unique Filename
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

        // 3. Save to Disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await input.File.CopyToAsync(stream);
        }

        // 4. Update Database with the relative URL
        product.ImageUrl = $"/images/{fileName}";
        await _context.SaveChangesAsync();

        return Ok(new { ImageUrl = product.ImageUrl });
    }
}
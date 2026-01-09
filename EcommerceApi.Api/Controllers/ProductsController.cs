using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceApi.Api.Data;
using EcommerceApi.Api.Models;
using EcommerceApi.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using EcommerceApi.Api.Services;
using EcommerceApi.Api.Helpers;

namespace EcommerceApi.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ImageService _imageService;

    public ProductsController(ApplicationDbContext context, ImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] ProductParams productParams)
    {
        // 1. Start with a "Queryable" - this hasn't hit the DB yet!
    var query = _context.Products
        .Include(p => p.Category)
        .AsQueryable();

    // 2. Filter by Category if provided
    if (productParams.CategoryId.HasValue)
    {
        query = query.Where(p => p.CategoryId == productParams.CategoryId);
    }

    // 3. Search by Name
    if (!string.IsNullOrEmpty(productParams.Search))
    {
        query = query.Where(p => p.Name.ToLower().Contains(productParams.Search.ToLower()));
    }

    // 4. Sort logic
    query = productParams.Sort switch
    {
        "priceAsc" => query.OrderBy(p => p.Price),
        "priceDesc" => query.OrderByDescending(p => p.Price),
        _ => query.OrderBy(p => p.Name) // Default sort
    };

    // Get total count BEFORE pagination
    var totalItems = await query.CountAsync();

    // 5. Apply Pagination
    var products = await query
        .Skip(productParams.PageSize * (productParams.PageNumber - 1))
        .Take(productParams.PageSize)
        .ToListAsync();

    // Calculate total pages
    var totalPages = (int)Math.Ceiling(totalItems / (double)productParams.PageSize);

    // Create the Header Object
    var paginationHeader = new PaginationHeader(
        productParams.PageNumber, 
        productParams.PageSize, 
        totalItems, 
        totalPages);

    // 5. Add the header to the Response
    Response.Headers.Append("Pagination", System.Text.Json.JsonSerializer.Serialize(paginationHeader));
    // Important for CORS (allows frontend to read the custom header)
    Response.Headers.Append("Access-Control-Expose-Headers", "Pagination");

    // 6. Map to DTO
    var productDtos = products.Select(p => new ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        StockQuantity = p.StockQuantity,
        ImageUrl = p.ImageUrl,
        CategoryName = p.Category?.Name ?? "No Category"
    });

    return Ok(productDtos);
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        // var product = await _context.Products.FindAsync(id);
        var product = await _context.Products
        .Include(p => p.Category) 
        .FirstOrDefaultAsync(p => p.Id == id);

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
            ImageUrl = product.ImageUrl,
            CategoryName = product.Category?.Name ?? "No Category"
        };
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> PostProduct([FromForm] ProductCreateDto productCreateDto, IFormFile? imageFile)
    {
        // 1. Map CreateDto -> Model (Preparing for Database)
        var product = new Product
        {
            Name = productCreateDto.Name,
            Description = productCreateDto.Description,
            Price = productCreateDto.Price,
            CategoryId = productCreateDto.CategoryId,
            StockQuantity = productCreateDto.StockQuantity
        };

        if (imageFile != null)
        {
            // Save image and get the path back
            product.ImageUrl = await _imageService.SaveImageAsync(imageFile);
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var productWithCategory = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == product.Id);

        if (productWithCategory == null) return BadRequest("Failed to create product");

        // 2. Map Model -> ProductDto (Preparing for User Response)
        var responseDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            CategoryName = productWithCategory.Category?.Name ?? "No Category"
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
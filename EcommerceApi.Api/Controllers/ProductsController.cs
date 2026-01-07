using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceApi.Api.Data;
using EcommerceApi.Api.Models;

namespace EcommerceApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // This makes the URL: api/products
public class ProductsController: ControllerBase
{
    private readonly ApplicationDbContext _context;

    // We "Inject" the database context here
    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products.ToListAsync();
    }

    // 2. GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }

    // 3. POST: api/products
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
}
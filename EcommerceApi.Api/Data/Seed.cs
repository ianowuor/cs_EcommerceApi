using EcommerceApi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Api.Data;

public static class Seed
{
    public static async Task SeedData(ApplicationDbContext context)
    {
        // Seed Categories first (because Products depend on them)
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Electronics" },
                new Category { Name = "Home Office" },
                new Category { Name = "Clothing" }
            };

            await context.Categories.AddRangeAsync();
            await context.SaveChangesAsync();
        }

        // Seed Products 
        if (!await context.Products.AnyAsync())
        {
            var electronics = await context.Categories.FirstAsync(c => c.Name == "Electronics");

            var products = new List<Product>
            {
                new Product
                {
                    Name = "Mechanical Keyboard", 
                    Description = "RGB Backlit keyboard with blue switches", 
                    Price = 89.99m,
                    CategoryId = electronics.Id,
                    ImageUrl = "/images/default-keyboard.jpg"
                },
                new Product
                {
                    Name = "Wireless Mouse", 
                    Description = "Ergonomic 2.4GHz mouse", 
                    Price = 25.50m,
                    CategoryId = electronics.Id,
                    ImageUrl = "/images/default-mouse.jpg"
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
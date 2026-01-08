using EcommerceApi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Api.Data;

public static class Seed
{
    public static async Task SeedData(ApplicationDbContext context)
    {
        Console.WriteLine("--> Seeding process started...");

        // 1. Check if Categories exist
        if (await context.Categories.AnyAsync())
        {
            Console.WriteLine("--> Categories already exist, skipping.");
        }
        else
        {
            Console.WriteLine("--> Seeding Categories...");
            var electronics = new Category { Name = "Electronics" };
            var homeOffice = new Category { Name = "Home Office" };
            var clothing = new Category { Name = "Clothing" };

            var categories = new List<Category> { electronics, homeOffice, clothing };

            await context.Categories.AddRangeAsync(categories);
            // WE MUST SAVE HERE so the IDs are generated
            await context.SaveChangesAsync(); 
            Console.WriteLine("--> Categories saved successfully.");
        }

        // 2. Check if Products exist
        if (await context.Products.AnyAsync())
        {
            Console.WriteLine("--> Products already exist, skipping.");
        }
        else
        {
            Console.WriteLine("--> Seeding Products...");
            
            // Look up the category we just saved to ensure we have the real ID
            var electronicsCat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics");

            if (electronicsCat != null)
            {
                var products = new List<Product>
                {
                    new Product 
                    { 
                        Name = "Mechanical Keyboard", 
                        Description = "RGB Backlit keyboard with blue switches", 
                        Price = 89.99m,
                        CategoryId = electronicsCat.Id,
                        ImageUrl = "/images/default-keyboard.jpg"
                    },
                    new Product 
                    { 
                        Name = "Wireless Mouse", 
                        Description = "Ergonomic 2.4GHz mouse", 
                        Price = 25.50m,
                        CategoryId = electronicsCat.Id,
                        ImageUrl = "/images/default-mouse.jpg"
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
                Console.WriteLine("--> Products saved successfully.");
            }
        }
        
        Console.WriteLine("--> Data Seeding Complete.");
    }
}
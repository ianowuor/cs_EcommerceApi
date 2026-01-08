using System.Collections.Generic;

namespace EcommerceApi.Api.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation Property: One category has many products
    public List<Product> Products { get; set; } = new();
}
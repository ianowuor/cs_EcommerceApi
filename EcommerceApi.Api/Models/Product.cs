namespace EcommerceApi.Api.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; } // For your image upload task later
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
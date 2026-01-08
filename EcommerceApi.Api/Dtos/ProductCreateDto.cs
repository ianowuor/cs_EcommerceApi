using System.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Api.Dtos;

public class ProductCreateDto
{
    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000.")]
    public decimal Price { get; set; }
    public int StockQuantity { get; set; } 

    [Required(ErrorMessage = "A Category ID is required.")]
    public int CategoryId { get; set; }
}
using System.Net.Http.Headers;

namespace EcommerceApi.Api.Dtos;

public class ProductCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuatity { get; set; }
}
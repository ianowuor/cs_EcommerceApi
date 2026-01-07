using Microsoft.AspNetCore.Http;

namespace EcommerceApi.Api.Dtos;

public class UploadImageDto
{
    public IFormFile File { get; set; } = null!;
}
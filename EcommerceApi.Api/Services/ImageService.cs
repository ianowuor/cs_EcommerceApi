namespace EcommerceApi.Api.Services;

public class ImageService
{
    private readonly IWebHostEnvironment _env;

    public ImageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string?> SaveImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) return null;

        // 1. Create a unique filename (e.g., 5f3a...jpg) so users don't overwrite each other
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        
        // 2. Combine paths to get: wwwroot/images/products/filename.jpg
        var filePath = Path.Combine(_env.WebRootPath, "images", "products", fileName);

        // 3. Save the actual file to the disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 4. Return the URL path for the database (e.g., /images/products/unique-name.jpg)
        return $"/images/products/{fileName}";
    }
}
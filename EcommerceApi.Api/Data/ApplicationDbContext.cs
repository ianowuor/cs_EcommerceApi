using Microsoft.EntityFrameworkCore;
using EcommerceApi.Api.Models;

namespace EcommerceApi.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<Product> Products { get; set; }
}
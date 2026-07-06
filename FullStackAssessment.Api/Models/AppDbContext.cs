using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FullStackAssessment.Api.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Seed Categories / Product Types
        modelBuilder.Entity<ProductType>().HasData(
            new ProductType { Id = 1, Name = "Electronics" },
            new ProductType { Id = 2, Name = "Furniture" },
            new ProductType { Id = 3, Name = "Miscellaneous" }
        );

        // 2. Seed Products based on test dataset
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Graphics Card",
                Price = 899.99m,
                Description = "High-performance GPU for gaming and rendering.",
                ProductTypeId = 1
            },
            new Product
            {
                Id = 2,
                Name = "Office Chair",
                Price = 199.99m,
                Description = "Ergonomic office chair with lumbar support.",
                ProductTypeId = 2
            },
            new Product
            {
                Id = 3,
                Name = "Keyboard",
                Price = 49.99m,
                Description = "Mechanical RGB tactile keyboard.",
                ProductTypeId = 1
            },
            new Product
            {
                Id = 4,
                Name = "Gaming Mouse",
                Price = 59.99m,
                Description = "High-precision wireless gaming mouse.",
                ProductTypeId = 1
            },
            new Product
            {
                Id = 5,
                Name = "Obsolete Item",
                Price = 5.00m,
                Description = "Legacy hardware inventory item.",
                ProductTypeId = 3
            },
            new Product
            {
                Id = 999,
                Name = "Ghost Product",
                Price = 0.00m,
                Description = "Generic placeholder data for system testing.",
                ProductTypeId = 3
            }
        );
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

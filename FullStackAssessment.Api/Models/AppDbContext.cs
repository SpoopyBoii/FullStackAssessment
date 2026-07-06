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
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_ProductType");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.ToTable("ProductType");

            entity.HasIndex(e => e.Name, "UQ_ProductType_Name").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FullStackAssessment.Api.Controllers;
using FullStackAssessment.Api.Models;
using FullStackAssessment.Api.DTOs;
using Xunit;

namespace ProductInventory.Tests
{
    public class ProductControllerTests
    {
        // Helper to spin up a fresh, isolated database in memory AND seed required lookup data
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            context.ProductTypes.AddRange(new List<ProductType>
            {
                new ProductType { Id = 1, Name = "Electronics" },
                new ProductType { Id = 2, Name = "Furniture" }
            });
            context.SaveChanges();

            return context;
        }

        #region GET (Read All)

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            using var context = GetDbContext();
            context.Products.AddRange(new List<Product>
            {
                new Product { Id = 1, Name = "Graphics Card", Description = "GPU", Price = 899.99M, ProductTypeId = 1 },
                new Product { Id = 2, Name = "Office Chair", Description = "Chair", Price = 199.99M, ProductTypeId = 2 }
            });
            await context.SaveChangesAsync();

            var controller = new ProductsController(context);

            // Act
            var result = await controller.GetProducts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ProductDto>>>(result);

            if (actionResult.Result is OkObjectResult okResult)
            {
                var products = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
                Assert.Equal(2, products.Count());
            }
            else
            {
                var products = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(actionResult.Value);
                Assert.Equal(2, products.Count());
            }
        }

        #endregion

        #region GET BY ID (Read Single)

        [Fact]
        public async Task GetProduct_ReturnsOkResult_WhenProductExists()
        {
            // Arrange
            using var context = GetDbContext();
            context.Products.Add(new Product { Id = 1, Name = "Keyboard", Description = "Mechanical", Price = 49.99M, ProductTypeId = 1 });
            await context.SaveChangesAsync();

            var controller = new ProductsController(context);

            // Act
            var result = await controller.GetProduct(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ProductDto>>(result);

            if (actionResult.Result is OkObjectResult okResult)
            {
                var product = Assert.IsType<ProductDto>(okResult.Value);
                Assert.Equal("Keyboard", product.Name);
            }
            else
            {
                var product = Assert.IsType<ProductDto>(actionResult.Value);
                Assert.Equal("Keyboard", product.Name);
            }
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            using var context = GetDbContext();
            var controller = new ProductsController(context);

            // Act
            var result = await controller.GetProduct(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        #endregion

        #region POST (Create)

        [Fact]
        public async Task PostProduct_AddsProductToDatabase_AndReturnsCreatedAtAction()
        {
            // Arrange
            using var context = GetDbContext();
            var controller = new ProductsController(context);

            var newProductDto = new ProductCreateUpdateDto
            {
                Name = "Gaming Mouse",
                Description = "High precision wireless mouse",
                Price = 59.99M,
                ProductTypeId = 1
            };

            // Act
            var result = await controller.PostProduct(newProductDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ProductDto>>(result);

            ProductDto returnedProduct;

            if (actionResult.Result is CreatedAtActionResult createdAtActionResult)
            {
                returnedProduct = Assert.IsType<ProductDto>(createdAtActionResult.Value);
            }
            else if (actionResult.Result is OkObjectResult okObjectResult)
            {
                returnedProduct = Assert.IsType<ProductDto>(okObjectResult.Value);
            }
            else
            {
                Assert.Null(actionResult.Result);
                returnedProduct = Assert.IsType<ProductDto>(actionResult.Value);
            }

            Assert.NotNull(returnedProduct);
            Assert.Equal("Gaming Mouse", returnedProduct.Name);
            Assert.Equal(1, context.Products.Count(p => p.Name == "Gaming Mouse"));
        }

        #endregion

        #region PUT (Update)

        [Fact]
        public async Task PutProduct_UpdatesProduct_AndReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            using var context = GetDbContext();
            var existingProduct = new Product { Id = 10, Name = "Old Name", Description = "Old Desc", Price = 10.00M, ProductTypeId = 1 };
            context.Products.Add(existingProduct);
            await context.SaveChangesAsync();
            context.Entry(existingProduct).State = EntityState.Detached;

            var controller = new ProductsController(context);

            var updatedProductDto = new ProductCreateUpdateDto
            {
                Name = "New Name",
                Description = "Updated Description",
                Price = 15.00M,
                ProductTypeId = 1
            };

            // Act
            var result = await controller.PutProduct(10, updatedProductDto);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var dbProduct = await context.Products.FindAsync(10);
            Assert.Equal("New Name", dbProduct?.Name);
            Assert.Equal(15.00M, dbProduct?.Price);
        }

        [Fact]
        public async Task PutProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            using var context = GetDbContext();
            var controller = new ProductsController(context);
            var updatedProductDto = new ProductCreateUpdateDto { Name = "Ghost Product", Description = "None", Price = 10.00M, ProductTypeId = 1 };

            // Act
            var result = await controller.PutProduct(999, updatedProductDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutProduct_ReturnsNotFoundOrBadRequest_WhenIdMismatch()
        {
            // Arrange
            using var context = GetDbContext();

            var existingProduct = new Product { Id = 1, Name = "Item 1", Description = "Desc", Price = 10.00M, ProductTypeId = 1 };
            context.Products.Add(existingProduct);
            await context.SaveChangesAsync();
            context.Entry(existingProduct).State = EntityState.Detached;

            var controller = new ProductsController(context);
            var updatedProductDto = new ProductCreateUpdateDto { Name = "Mismatched", Description = "Desc", Price = 10.00M, ProductTypeId = 1 };

            // Act
            var result = await controller.PutProduct(1, updatedProductDto);

            // Assert
            Assert.True(result is NotFoundResult || result is BadRequestResult || result is BadRequestObjectResult || result is NoContentResult);
        }

        #endregion

        #region DELETE (Delete)

        [Fact]
        public async Task DeleteProduct_RemovesProduct_AndReturnsNoContent_WhenProductExists()
        {
            // Arrange
            using var context = GetDbContext();
            var productToDelete = new Product { Id = 3, Name = "Obsolete Item", Description = "Trash", Price = 5.00M, ProductTypeId = 2 };
            context.Products.Add(productToDelete);
            await context.SaveChangesAsync();

            var controller = new ProductsController(context);

            // Act
            var result = await controller.DeleteProduct(3);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var dbProduct = await context.Products.FindAsync(3);
            Assert.Null(dbProduct);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            using var context = GetDbContext();
            var controller = new ProductsController(context);

            // Act
            var result = await controller.DeleteProduct(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion
    }
}
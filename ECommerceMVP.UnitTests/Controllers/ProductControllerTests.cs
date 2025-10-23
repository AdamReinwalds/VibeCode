using ECommerceMVP.Api.Controllers;
using ECommerceMVP.Application.DTOs;
using ECommerceMVP.Application.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ECommerceMVP.UnitTests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _controller = new ProductsController(_productServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResultWithProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = "prod1", Name = "Product 1", Price = 10.99m },
            new ProductDto { Id = "prod2", Name = "Product 2", Price = 20.99m }
        };

        _productServiceMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeOfType<List<ProductDto>>().Subject;
        returnedProducts.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task GetById_ExistingProduct_ReturnsOkResult()
    {
        // Arrange
        var productId = "prod1";
        var product = new ProductDto
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description",
            Price = 29.99m
        };

        _productServiceMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(productId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Should().BeEquivalentTo(product);
    }

    [Fact]
    public async Task GetById_NonExistingProduct_ReturnsNotFound()
    {
        // Arrange
        var productId = "nonexistent";
        _productServiceMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _controller.GetById(productId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var request = new CreateProductDto
        {
            Name = "New Product",
            Description = "New Description",
            Price = 49.99m,
            Category = "Electronics"
        };

        var createdProduct = new ProductDto
        {
            Id = "new-prod-id",
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category
        };

        _productServiceMock.Setup(x => x.CreateAsync(request))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be("GetById");
        createdResult.RouteValues!["id"].Should().Be("new-prod-id");
        var returnedProduct = createdResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Should().BeEquivalentTo(createdProduct);
    }

    [Fact]
    public async Task Create_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateProductDto
        {
            Name = "", // Invalid: empty name
            Description = "Description",
            Price = 49.99m,
            Category = "Electronics"
        };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_ExistingProduct_ReturnsNoContent()
    {
        // Arrange
        var productId = "prod1";
        var request = new UpdateProductDto
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 59.99m
        };

        _productServiceMock.Setup(x => x.UpdateAsync(productId, request))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Update(productId, request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_NonExistingProduct_ReturnsNotFound()
    {
        // Arrange
        var productId = "nonexistent";
        var request = new UpdateProductDto
        {
            Name = "Updated Product",
            Price = 59.99m
        };

        _productServiceMock.Setup(x => x.UpdateAsync(productId, request))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Update(productId, request);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ExistingProduct_ReturnsNoContent()
    {
        // Arrange
        var productId = "prod1";
        _productServiceMock.Setup(x => x.DeleteAsync(productId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(productId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_NonExistingProduct_ReturnsNotFound()
    {
        // Arrange
        var productId = "nonexistent";
        _productServiceMock.Setup(x => x.DeleteAsync(productId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(productId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetByCategory_ReturnsProductsInCategory()
    {
        // Arrange
        var category = "Electronics";
        var products = new List<ProductDto>
        {
            new ProductDto { Id = "prod1", Name = "Laptop", Category = category, Price = 999.99m },
            new ProductDto { Id = "prod2", Name = "Phone", Category = category, Price = 699.99m }
        };

        _productServiceMock.Setup(x => x.GetByCategoryAsync(category))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetByCategory(category);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeOfType<List<ProductDto>>().Subject;
        returnedProducts.Should().BeEquivalentTo(products);
        returnedProducts.Should().AllSatisfy(p => p.Category.Should().Be(category));
    }

    [Fact]
    public async Task GetByCategory_EmptyCategory_ReturnsEmptyList()
    {
        // Arrange
        var category = "NonExistent";
        _productServiceMock.Setup(x => x.GetByCategoryAsync(category))
            .ReturnsAsync(new List<ProductDto>());

        // Act
        var result = await _controller.GetByCategory(category);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeOfType<List<ProductDto>>().Subject;
        returnedProducts.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_InvalidId_ReturnsBadRequest()
    {
        // Arrange
        var invalidId = "";

        // Act
        var result = await _controller.GetById(invalidId);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Update_InvalidId_ReturnsBadRequest()
    {
        // Arrange
        var invalidId = "";
        var request = new UpdateProductDto { Name = "Updated" };

        // Act
        var result = await _controller.Update(invalidId, request);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Delete_InvalidId_ReturnsBadRequest()
    {
        // Arrange
        var invalidId = "";

        // Act
        var result = await _controller.Delete(invalidId);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task GetByCategory_InvalidCategory_ReturnsBadRequest()
    {
        // Arrange
        var invalidCategory = "";

        // Act
        var result = await _controller.GetByCategory(invalidCategory);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [InlineData("Electronics")]
    [InlineData("Books")]
    [InlineData("Clothing")]
    public async Task GetByCategory_VariousCategories_ReturnsCorrectResults(string category)
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = "prod1", Name = "Product", Category = category, Price = 29.99m }
        };

        _productServiceMock.Setup(x => x.GetByCategoryAsync(category))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetByCategory(category);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeOfType<List<ProductDto>>().Subject;
        returnedProducts.Should().HaveCount(1);
        returnedProducts[0].Category.Should().Be(category);
    }

    [Fact]
    public async Task Create_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Description",
            Price = 29.99m,
            Category = "Test"
        };

        _productServiceMock.Setup(x => x.CreateAsync(request))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.Create(request));
    }

    [Fact]
    public async Task Update_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var productId = "prod1";
        var request = new UpdateProductDto { Name = "Updated" };

        _productServiceMock.Setup(x => x.UpdateAsync(productId, request))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.Update(productId, request));
    }

    [Fact]
    public async Task GetById_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var productId = "prod1";

        _productServiceMock.Setup(x => x.GetByIdAsync(productId))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.GetById(productId));
    }
}
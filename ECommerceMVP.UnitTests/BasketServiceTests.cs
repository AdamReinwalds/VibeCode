using ECommerceMVP.Application.DTOs;
using ECommerceMVP.Application.Services;
using ECommerceMVP.Domain.Entities;
using ECommerceMVP.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerceMVP.UnitTests;

public class BasketServiceTests
{
    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly BasketService _basketService;

    public BasketServiceTests()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _basketService = new BasketService(_basketRepositoryMock.Object, _productRepositoryMock.Object);
    }

    [Fact]
    public async Task GetBasketAsync_UserHasBasket_ShouldReturnBasketDto()
    {
        // Arrange
        var userId = "user123";
        var product = new Product { Id = "prod1", Name = "Test Product", Price = 29.99m };
        var basket = new Basket
        {
            Id = "basket123",
            UserId = userId,
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = "prod1", Quantity = 2, Price = 29.99m }
            }
        };

        _basketRepositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(basket);
        _productRepositoryMock.Setup(x => x.GetByIdAsync("prod1")).ReturnsAsync(product);

        // Act
        var result = await _basketService.GetBasketAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.BasketId.Should().Be("basket123");
        result.Items.Should().HaveCount(1);
        result.Items[0].ProductId.Should().Be("prod1");
        result.Items[0].Name.Should().Be("Test Product");
        result.Items[0].Quantity.Should().Be(2);
        result.Items[0].TotalPrice.Should().Be(59.98m);
        result.TotalAmount.Should().Be(59.98m);
    }

    [Fact]
    public async Task GetBasketAsync_UserHasNoBasket_ShouldReturnEmptyBasketDto()
    {
        // Arrange
        var userId = "user123";
        _basketRepositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync((Basket?)null);

        // Act
        var result = await _basketService.GetBasketAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.BasketId.Should().BeEmpty();
        result.Items.Should().BeEmpty();
        result.TotalAmount.Should().Be(0);
    }

    [Fact]
    public async Task AddToBasketAsync_ProductExistsAndInStock_ShouldAddItemAndReturnDto()
    {
        // Arrange
        var userId = "user123";
        var request = new AddToBasketRequest { ProductId = "prod1", Quantity = 3 };
        var product = new Product { Id = "prod1", Name = "Test Product", Price = 19.99m, Stock = 10 };

        _productRepositoryMock.Setup(x => x.GetByIdAsync("prod1")).ReturnsAsync(product);
        _basketRepositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync((Basket?)null);

        Basket? capturedBasket = null;
        _basketRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Basket>()))
            .Callback<Basket>(b => capturedBasket = b)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _basketService.AddToBasketAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.Should().Be("prod1");
        result.Name.Should().Be("Test Product");
        result.Quantity.Should().Be(3);
        result.TotalPrice.Should().Be(59.97m);

        capturedBasket.Should().NotBeNull();
        capturedBasket!.UserId.Should().Be(userId);
        capturedBasket.Items.Should().HaveCount(1);
        capturedBasket.Items[0].ProductId.Should().Be("prod1");
        capturedBasket.Items[0].Quantity.Should().Be(3);
    }

    [Fact]
    public async Task AddToBasketAsync_ProductNotFound_ShouldThrowException()
    {
        // Arrange
        var userId = "user123";
        var request = new AddToBasketRequest { ProductId = "nonexistent", Quantity = 1 };

        _productRepositoryMock.Setup(x => x.GetByIdAsync("nonexistent")).ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _basketService.AddToBasketAsync(userId, request));
    }

    [Fact]
    public async Task AddToBasketAsync_InsufficientStock_ShouldThrowException()
    {
        // Arrange
        var userId = "user123";
        var request = new AddToBasketRequest { ProductId = "prod1", Quantity = 5 };
        var product = new Product { Id = "prod1", Name = "Test Product", Price = 19.99m, Stock = 3 };

        _productRepositoryMock.Setup(x => x.GetByIdAsync("prod1")).ReturnsAsync(product);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _basketService.AddToBasketAsync(userId, request));
    }

    [Fact]
    public async Task AddToBasketAsync_ItemAlreadyInBasket_ShouldUpdateQuantity()
    {
        // Arrange
        var userId = "user123";
        var request = new AddToBasketRequest { ProductId = "prod1", Quantity = 2 };
        var product = new Product { Id = "prod1", Name = "Test Product", Price = 19.99m, Stock = 10 };
        var basket = new Basket
        {
            Id = "basket123",
            UserId = userId,
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = "prod1", Quantity = 1, Price = 19.99m }
            }
        };

        _productRepositoryMock.Setup(x => x.GetByIdAsync("prod1")).ReturnsAsync(product);
        _basketRepositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(basket);

        // Act
        var result = await _basketService.AddToBasketAsync(userId, request);

        // Assert
        result.Quantity.Should().Be(3); // 1 + 2
        result.TotalPrice.Should().Be(59.97m); // 3 * 19.99

        _basketRepositoryMock.Verify(x => x.UpdateAsync(basket), Times.Once);
    }

    [Fact]
    public async Task RemoveFromBasketAsync_ItemExists_ShouldRemoveItem()
    {
        // Arrange
        var userId = "user123";
        var productId = "prod1";
        var basket = new Basket
        {
            Id = "basket123",
            UserId = userId,
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = "prod1", Quantity = 2, Price = 19.99m },
                new BasketItem { ProductId = "prod2", Quantity = 1, Price = 29.99m }
            }
        };

        _basketRepositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(basket);

        // Act
        await _basketService.RemoveFromBasketAsync(userId, productId);

        // Assert
        basket.Items.Should().HaveCount(1);
        basket.Items[0].ProductId.Should().Be("prod2");

        _basketRepositoryMock.Verify(x => x.UpdateAsync(basket), Times.Once);
    }

    [Fact]
    public async Task RemoveFromBasketAsync_ItemNotFound_ShouldThrowException()
    {
        // Arrange
        var userId = "user123";
        var productId = "nonexistent";
        var basket = new Basket
        {
            Id = "basket123",
            UserId = userId,
            Items = new List<BasketItem>()
        };

        _basketRepositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(basket);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _basketService.RemoveFromBasketAsync(userId, productId));
    }

    [Fact]
    public async Task ClearBasketAsync_BasketExists_ShouldClearItems()
    {
        // Arrange
        var userId = "user123";
        var basket = new Basket
        {
            Id = "basket123",
            UserId = userId,
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = "prod1", Quantity = 2, Price = 19.99m }
            }
        };

        _basketRepositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(basket);

        // Act
        await _basketService.ClearBasketAsync(userId);

        // Assert
        basket.Items.Should().BeEmpty();

        _basketRepositoryMock.Verify(x => x.UpdateAsync(basket), Times.Once);
    }
}
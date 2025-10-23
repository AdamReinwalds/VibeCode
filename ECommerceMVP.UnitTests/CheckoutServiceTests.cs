using ECommerceMVP.Application.DTOs;
using ECommerceMVP.Application.Services;
using ECommerceMVP.Domain.Entities;
using ECommerceMVP.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerceMVP.UnitTests;

public class CheckoutServiceTests
{
    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly CheckoutService _checkoutService;

    public CheckoutServiceTests()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _checkoutService = new CheckoutService(
            _basketRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object);
    }

    [Fact]
    public async Task CheckoutAsync_ValidBasketAndStock_ShouldCreateOrderAndClearBasket()
    {
        // Arrange
        var userId = "user123";
        var request = new CheckoutRequest
        {
            ShippingAddress = new ShippingAddressDto
            {
                Street = "123 Main St",
                City = "Test City",
                PostalCode = "12345",
                Country = "Test Country"
            },
            PaymentMethod = "Credit Card"
        };

        var product1 = new Product { Id = "prod1", Name = "Product 1", Price = 19.99m, Stock = 10 };
        var product2 = new Product { Id = "prod2", Name = "Product 2", Price = 29.99m, Stock = 5 };

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
        _productRepositoryMock.Setup(x => x.GetByIdAsync("prod1")).ReturnsAsync(product1);
        _productRepositoryMock.Setup(x => x.GetByIdAsync("prod2")).ReturnsAsync(product2);

        Order? capturedOrder = null;
        _orderRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Order>()))
            .Callback<Order>(o => {
                capturedOrder = o;
                // Simulate MongoDB setting the ID
                if (string.IsNullOrEmpty(o.Id))
                {
                    o.Id = "507f1f77bcf86cd799439011";
                }
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _checkoutService.CheckoutAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(69.97m); // (2 * 19.99) + (1 * 29.99)
        result.Status.Should().Be("Paid");

        capturedOrder.Should().NotBeNull();
        capturedOrder!.UserId.Should().Be(userId);
        capturedOrder.Items.Should().HaveCount(2);
        capturedOrder.TotalAmount.Should().Be(69.97m);
        capturedOrder.Status.Should().Be(OrderStatus.Paid);
        capturedOrder.ShippingAddress.Street.Should().Be(request.ShippingAddress.Street);
        capturedOrder.PaymentMethod.Should().Be(request.PaymentMethod);
        // Order ID will be set by MongoDB when saved, so we check it's not null after creation
        capturedOrder.Id.Should().NotBeNullOrEmpty();

        _basketRepositoryMock.Verify(x => x.UpdateAsync(basket), Times.Once);
        basket.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task CheckoutAsync_EmptyBasket_ShouldThrowException()
    {
        // Arrange
        var userId = "user123";
        var request = new CheckoutRequest
        {
            ShippingAddress = new ShippingAddressDto
            {
                Street = "123 Main St",
                City = "Test City",
                PostalCode = "12345",
                Country = "Test Country"
            },
            PaymentMethod = "Credit Card"
        };

        var basket = new Basket
        {
            Id = "basket123",
            UserId = userId,
            Items = new List<BasketItem>() // Empty basket
        };

        _basketRepositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(basket);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _checkoutService.CheckoutAsync(userId, request));
    }

    [Fact]
    public async Task CheckoutAsync_ProductNotFound_ShouldThrowException()
    {
        // Arrange
        var userId = "user123";
        var request = new CheckoutRequest
        {
            ShippingAddress = new ShippingAddressDto
            {
                Street = "123 Main St",
                City = "Test City",
                PostalCode = "12345",
                Country = "Test Country"
            },
            PaymentMethod = "Credit Card"
        };

        var basket = new Basket
        {
            Id = "basket123",
            UserId = userId,
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = "nonexistent", Quantity = 1, Price = 19.99m }
            }
        };

        _basketRepositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(basket);
        _productRepositoryMock.Setup(x => x.GetByIdAsync("nonexistent")).ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _checkoutService.CheckoutAsync(userId, request));
    }

    [Fact]
    public async Task CheckoutAsync_InsufficientStock_ShouldThrowException()
    {
        // Arrange
        var userId = "user123";
        var request = new CheckoutRequest
        {
            ShippingAddress = new ShippingAddressDto
            {
                Street = "123 Main St",
                City = "Test City",
                PostalCode = "12345",
                Country = "Test Country"
            },
            PaymentMethod = "Credit Card"
        };

        var product = new Product { Id = "prod1", Name = "Product 1", Price = 19.99m, Stock = 1 };
        var basket = new Basket
        {
            Id = "basket123",
            UserId = userId,
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = "prod1", Quantity = 5, Price = 19.99m } // More than stock
            }
        };

        _basketRepositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(basket);
        _productRepositoryMock.Setup(x => x.GetByIdAsync("prod1")).ReturnsAsync(product);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _checkoutService.CheckoutAsync(userId, request));
    }

    [Fact]
    public async Task GetUserOrdersAsync_ShouldReturnOrdersWithPagination()
    {
        // Arrange
        var userId = "user123";
        var orders = new List<Order>
        {
            new Order
            {
                Id = "order1",
                UserId = userId,
                TotalAmount = 49.98m,
                Status = OrderStatus.Paid,
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = "prod1", Quantity = 2, Price = 19.99m }
                }
            }
        };

        var product = new Product { Id = "prod1", Name = "Product 1" };
        _orderRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, 1, 10)).ReturnsAsync(orders);
        _orderRepositoryMock.Setup(x => x.GetTotalCountByUserIdAsync(userId)).ReturnsAsync(1);
        _productRepositoryMock.Setup(x => x.GetByIdAsync("prod1")).ReturnsAsync(product);

        // Act
        var result = await _checkoutService.GetUserOrdersAsync(userId, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Orders.Should().HaveCount(1);
        result.Orders[0].OrderId.Should().Be("order1");
        result.Orders[0].TotalAmount.Should().Be(49.98m);
        result.Orders[0].Items.Should().HaveCount(1);
        result.Orders[0].Items[0].Name.Should().Be("Product 1");
        result.Pagination.Total.Should().Be(1);
        result.Pagination.Page.Should().Be(1);
        result.Pagination.Limit.Should().Be(10);
    }
}
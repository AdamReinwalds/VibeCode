using ECommerceMVP.Application.DTOs;
using ECommerceMVP.Application.Services.Interfaces;
using ECommerceMVP.Domain.Entities;
using ECommerceMVP.Domain.Repositories;

namespace ECommerceMVP.Application.Services;

public class CheckoutService : ICheckoutService
{
    private readonly IBasketRepository _basketRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public CheckoutService(
        IBasketRepository basketRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _basketRepository = basketRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<OrderSummaryDto> CheckoutAsync(string userId, CheckoutRequest request)
    {
        // Get user's basket
        var basket = await _basketRepository.GetByUserIdAsync(userId);
        if (basket == null || !basket.Items.Any())
        {
            throw new InvalidOperationException("Basket is empty");
        }

        // Validate stock availability (this is a simple check, in production you'd use transactions)
        foreach (var item in basket.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product {item.ProductId} not found");
            }
            if (product.Stock < item.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}");
            }
        }

        // Simulate payment processing (in real app, integrate with payment gateway)
        await SimulatePaymentProcessingAsync();

        // Create order
        var order = new Order
        {
            UserId = userId,
            Items = basket.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList(),
            TotalAmount = basket.Items.Sum(i => i.Quantity * i.Price),
            Status = OrderStatus.Paid,
            ShippingAddress = new ShippingAddress
            {
                Street = request.ShippingAddress.Street,
                City = request.ShippingAddress.City,
                PostalCode = request.ShippingAddress.PostalCode,
                Country = request.ShippingAddress.Country
            },
            PaymentMethod = request.PaymentMethod,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _orderRepository.CreateAsync(order);

        // Clear the basket after successful checkout
        basket.Items.Clear();
        basket.UpdatedAt = DateTime.UtcNow;
        await _basketRepository.UpdateAsync(basket);

        return new OrderSummaryDto
        {
            OrderId = order.Id,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt
        };
    }

    public async Task<OrdersResponse> GetUserOrdersAsync(string userId, int page = 1, int limit = 10)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId, page, limit);
        var totalCount = await _orderRepository.GetTotalCountByUserIdAsync(userId);

        var orderDtos = new List<OrderDto>();
        foreach (var order in orders)
        {
            var orderDto = new OrderDto
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                Items = new List<OrderItemDto>()
            };

            // Populate order items with product names
            foreach (var item in order.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                orderDto.Items.Add(new OrderItemDto
                {
                    ProductId = item.ProductId,
                    Name = product?.Name ?? "Unknown Product",
                    Price = item.Price,
                    Quantity = item.Quantity
                });
            }

            orderDtos.Add(orderDto);
        }

        return new OrdersResponse
        {
            Orders = orderDtos,
            Pagination = new PaginationDto
            {
                Page = page,
                Limit = limit,
                Total = (int)totalCount
            }
        };
    }

    private async Task SimulatePaymentProcessingAsync()
    {
        // Simulate payment processing delay and potential failure
        await Task.Delay(100); // Simulate processing time

        // In a real application, this would integrate with a payment gateway
        // For now, we'll randomly fail 5% of the time to simulate payment failures
        if (Random.Shared.Next(100) < 5)
        {
            throw new InvalidOperationException("Payment processing failed");
        }
    }
}
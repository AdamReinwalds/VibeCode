using ECommerceMVP.Application.DTOs;
using ECommerceMVP.Application.Services.Interfaces;
using ECommerceMVP.Domain.Entities;
using ECommerceMVP.Domain.Repositories;

namespace ECommerceMVP.Application.Services;

public class BasketService : IBasketService
{
    private readonly IBasketRepository _basketRepository;
    private readonly IProductRepository _productRepository;

    public BasketService(IBasketRepository basketRepository, IProductRepository productRepository)
    {
        _basketRepository = basketRepository;
        _productRepository = productRepository;
    }

    public async Task<BasketDto> GetBasketAsync(string userId)
    {
        var basket = await _basketRepository.GetByUserIdAsync(userId);
        if (basket == null)
        {
            return new BasketDto
            {
                BasketId = string.Empty,
                Items = new List<BasketItemDto>(),
                TotalAmount = 0
            };
        }

        var basketDto = new BasketDto
        {
            BasketId = basket.Id,
            Items = new List<BasketItemDto>(),
            TotalAmount = 0
        };

        foreach (var item in basket.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                var itemDto = new BasketItemDto
                {
                    ProductId = item.ProductId,
                    Name = product.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    TotalPrice = item.Price * item.Quantity
                };
                basketDto.Items.Add(itemDto);
                basketDto.TotalAmount += itemDto.TotalPrice;
            }
        }

        return basketDto;
    }

    public async Task<BasketItemDto> AddToBasketAsync(string userId, AddToBasketRequest request)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
            throw new InvalidOperationException("Product not found");

        if (product.Stock < request.Quantity)
            throw new InvalidOperationException("Insufficient stock");

        var basket = await _basketRepository.GetByUserIdAsync(userId);
        if (basket == null)
        {
            basket = new Basket
            {
                UserId = userId,
                Items = new List<BasketItem>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _basketRepository.CreateAsync(basket);
        }

        var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            basket.Items.Add(new BasketItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Price = product.Price
            });
        }

        basket.UpdatedAt = DateTime.UtcNow;
        await _basketRepository.UpdateAsync(basket);

        return new BasketItemDto
        {
            ProductId = request.ProductId,
            Name = product.Name,
            Price = product.Price,
            Quantity = existingItem?.Quantity ?? request.Quantity,
            TotalPrice = product.Price * (existingItem?.Quantity ?? request.Quantity)
        };
    }

    public async Task RemoveFromBasketAsync(string userId, string productId)
    {
        var basket = await _basketRepository.GetByUserIdAsync(userId);
        if (basket == null)
            throw new InvalidOperationException("Basket not found");

        var itemToRemove = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (itemToRemove == null)
            throw new InvalidOperationException("Item not found in basket");

        basket.Items.Remove(itemToRemove);
        basket.UpdatedAt = DateTime.UtcNow;
        await _basketRepository.UpdateAsync(basket);
    }

    public async Task ClearBasketAsync(string userId)
    {
        var basket = await _basketRepository.GetByUserIdAsync(userId);
        if (basket != null)
        {
            basket.Items.Clear();
            basket.UpdatedAt = DateTime.UtcNow;
            await _basketRepository.UpdateAsync(basket);
        }
    }
}
using ECommerceMVP.Application.DTOs;

namespace ECommerceMVP.Application.Services.Interfaces;

public interface IBasketService
{
    Task<BasketDto> GetBasketAsync(string userId);
    Task<BasketItemDto> AddToBasketAsync(string userId, AddToBasketRequest request);
    Task RemoveFromBasketAsync(string userId, string productId);
    Task ClearBasketAsync(string userId);
}
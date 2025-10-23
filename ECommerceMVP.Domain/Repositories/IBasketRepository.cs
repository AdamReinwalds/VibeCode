using ECommerceMVP.Domain.Entities;

namespace ECommerceMVP.Domain.Repositories;

public interface IBasketRepository
{
    Task<Basket?> GetByUserIdAsync(string userId);
    Task<Basket?> GetByIdAsync(string id);
    Task CreateAsync(Basket basket);
    Task UpdateAsync(Basket basket);
    Task DeleteAsync(string id);
}
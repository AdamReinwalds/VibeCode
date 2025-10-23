using ECommerceMVP.Domain.Entities;

namespace ECommerceMVP.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(string id);
    Task<IEnumerable<Order>> GetByUserIdAsync(string userId, int page = 1, int limit = 10);
    Task<long> GetTotalCountByUserIdAsync(string userId);
    Task CreateAsync(Order order);
    Task UpdateAsync(Order order);
}
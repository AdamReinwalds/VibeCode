using ECommerceMVP.Domain.Entities;

namespace ECommerceMVP.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(string id);
    Task<IEnumerable<Product>> GetAllAsync(int page = 1, int limit = 10, string? category = null);
    Task<long> GetTotalCountAsync(string? category = null);
    Task CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(string id);
}
using ECommerceMVP.Domain.Entities;
using ECommerceMVP.Domain.Repositories;
using MongoDB.Driver;

namespace ECommerceMVP.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders;

    public OrderRepository(IMongoDatabase database)
    {
        _orders = database.GetCollection<Order>("Orders");

        // Create indexes
        var userIdIndex = Builders<Order>.IndexKeys.Ascending(o => o.UserId);
        var statusIndex = Builders<Order>.IndexKeys.Ascending(o => o.Status);
        var compoundIndex = Builders<Order>.IndexKeys.Combine(userIdIndex, statusIndex);

        _orders.Indexes.CreateOneAsync(new CreateIndexModel<Order>(userIdIndex));
        _orders.Indexes.CreateOneAsync(new CreateIndexModel<Order>(statusIndex));
        _orders.Indexes.CreateOneAsync(new CreateIndexModel<Order>(compoundIndex));
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        return await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId, int page = 1, int limit = 10)
    {
        var skip = (page - 1) * limit;

        return await _orders
            .Find(o => o.UserId == userId)
            .SortByDescending(o => o.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<long> GetTotalCountByUserIdAsync(string userId)
    {
        return await _orders.CountDocumentsAsync(o => o.UserId == userId);
    }

    public async Task CreateAsync(Order order)
    {
        await _orders.InsertOneAsync(order);
    }

    public async Task UpdateAsync(Order order)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, order.Id);
        await _orders.ReplaceOneAsync(filter, order);
    }
}
using ECommerceMVP.Domain.Entities;
using ECommerceMVP.Domain.Repositories;
using MongoDB.Driver;

namespace ECommerceMVP.Infrastructure.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IMongoCollection<Basket> _baskets;

    public BasketRepository(IMongoDatabase database)
    {
        _baskets = database.GetCollection<Basket>("Baskets");

        // Create index on userId for quick lookups
        var userIdIndex = Builders<Basket>.IndexKeys.Ascending(b => b.UserId);
        _baskets.Indexes.CreateOneAsync(new CreateIndexModel<Basket>(userIdIndex, new CreateIndexOptions { Unique = true }));
    }

    public async Task<Basket?> GetByUserIdAsync(string userId)
    {
        return await _baskets.Find(b => b.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<Basket?> GetByIdAsync(string id)
    {
        return await _baskets.Find(b => b.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Basket basket)
    {
        await _baskets.InsertOneAsync(basket);
    }

    public async Task UpdateAsync(Basket basket)
    {
        var filter = Builders<Basket>.Filter.Eq(b => b.Id, basket.Id);
        await _baskets.ReplaceOneAsync(filter, basket);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<Basket>.Filter.Eq(b => b.Id, id);
        await _baskets.DeleteOneAsync(filter);
    }
}
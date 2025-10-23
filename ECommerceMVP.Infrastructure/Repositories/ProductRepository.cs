using ECommerceMVP.Domain.Entities;
using ECommerceMVP.Domain.Repositories;
using MongoDB.Driver;

namespace ECommerceMVP.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _products;

    public ProductRepository(IMongoDatabase database)
    {
        _products = database.GetCollection<Product>("Products");

        // Create index on category for filtering
        var categoryIndex = Builders<Product>.IndexKeys.Ascending(p => p.Category);
        _products.Indexes.CreateOneAsync(new CreateIndexModel<Product>(categoryIndex));
    }

    public async Task<Product?> GetByIdAsync(string id)
    {
        return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetAllAsync(int page = 1, int limit = 10, string? category = null)
    {
        var filter = category != null
            ? Builders<Product>.Filter.Eq(p => p.Category, category)
            : Builders<Product>.Filter.Empty;

        var skip = (page - 1) * limit;

        return await _products
            .Find(filter)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<long> GetTotalCountAsync(string? category = null)
    {
        var filter = category != null
            ? Builders<Product>.Filter.Eq(p => p.Category, category)
            : Builders<Product>.Filter.Empty;

        return await _products.CountDocumentsAsync(filter);
    }

    public async Task CreateAsync(Product product)
    {
        await _products.InsertOneAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
        await _products.ReplaceOneAsync(filter, product);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        await _products.DeleteOneAsync(filter);
    }
}
using ECommerceMVP.Domain.Entities;
using ECommerceMVP.Domain.Repositories;
using MongoDB.Driver;

namespace ECommerceMVP.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("Users");

        // Create indexes
        var usernameIndex = Builders<User>.IndexKeys.Ascending(u => u.Username);
        var emailIndex = Builders<User>.IndexKeys.Ascending(u => u.Email);

        _users.Indexes.CreateOneAsync(new CreateIndexModel<User>(usernameIndex, new CreateIndexOptions { Unique = true }));
        _users.Indexes.CreateOneAsync(new CreateIndexModel<User>(emailIndex, new CreateIndexOptions { Unique = true }));
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string username, string email)
    {
        var filter = Builders<User>.Filter.Or(
            Builders<User>.Filter.Eq(u => u.Username, username),
            Builders<User>.Filter.Eq(u => u.Email, email)
        );

        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
        await _users.ReplaceOneAsync(filter, user);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        await _users.DeleteOneAsync(filter);
    }
}
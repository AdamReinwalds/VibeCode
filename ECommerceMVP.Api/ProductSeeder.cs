using ECommerceMVP.Domain.Entities;
using ECommerceMVP.Domain.Repositories;
using MongoDB.Driver;

namespace ECommerceMVP.Api;

public class ProductSeeder
{
    private readonly IMongoDatabase _database;
    private readonly IProductRepository _productRepository;

    public ProductSeeder(IMongoDatabase database, IProductRepository productRepository)
    {
        _database = database;
        _productRepository = productRepository;
    }

    public async Task SeedAsync()
    {
        Console.WriteLine("Starting product seeding...");

        var collection = _database.GetCollection<Product>("Products");

        // Check if products already exist
        var productCount = await collection.CountDocumentsAsync(FilterDefinition<Product>.Empty);
        Console.WriteLine($"Found {productCount} existing products in Products collection");

        if (productCount > 0)
        {
            Console.WriteLine("Products already exist, skipping seeding");
            return;
        }

        // Clean up any old data in lowercase collection if it exists
        try
        {
            var oldCollection = _database.GetCollection<Product>("products");
            var oldCount = await oldCollection.CountDocumentsAsync(FilterDefinition<Product>.Empty);
            if (oldCount > 0)
            {
                Console.WriteLine($"Found {oldCount} products in old 'products' collection, dropping it");
                await _database.DropCollectionAsync("products");
                Console.WriteLine("Old collection dropped successfully");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not drop old collection: {ex.Message}");
        }

        var products = new List<Product>
        {
            new Product
            {
                Name = "Classic White T-Shirt",
                Description = "Comfortable cotton t-shirt perfect for everyday wear",
                Price = 19.99m,
                ImageUrl = "https://example.com/images/white-tshirt.jpg",
                Stock = 50,
                Category = "T-Shirts",
                Size = "M",
                Color = "White",
                Material = "Cotton",
                Gender = "Unisex",
                Brand = "BasicWear",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Blue Denim Jeans",
                Description = "Classic blue jeans with perfect fit",
                Price = 79.99m,
                ImageUrl = "https://example.com/images/blue-jeans.jpg",
                Stock = 30,
                Category = "Jeans",
                Size = "32W x 34L",
                Color = "Blue",
                Material = "Denim",
                Gender = "Men",
                Brand = "DenimCo",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Black Leather Jacket",
                Description = "Stylish black leather jacket for any occasion",
                Price = 149.99m,
                ImageUrl = "https://example.com/images/leather-jacket.jpg",
                Stock = 15,
                Category = "Jackets",
                Size = "L",
                Color = "Black",
                Material = "Leather",
                Gender = "Unisex",
                Brand = "LeatherLux",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Red Summer Dress",
                Description = "Light and breezy summer dress",
                Price = 45.99m,
                ImageUrl = "https://example.com/images/red-dress.jpg",
                Stock = 25,
                Category = "Dresses",
                Size = "S",
                Color = "Red",
                Material = "Cotton",
                Gender = "Women",
                Brand = "SummerStyle",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Gray Hoodie",
                Description = "Comfortable hoodie for casual wear",
                Price = 39.99m,
                ImageUrl = "https://example.com/images/gray-hoodie.jpg",
                Stock = 40,
                Category = "Hoodies",
                Size = "XL",
                Color = "Gray",
                Material = "Cotton",
                Gender = "Unisex",
                Brand = "ComfortWear",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "White Sneakers",
                Description = "Classic white sneakers for everyday use",
                Price = 89.99m,
                ImageUrl = "https://example.com/images/white-sneakers.jpg",
                Stock = 35,
                Category = "Shoes",
                Size = "42",
                Color = "White",
                Material = "Synthetic",
                Gender = "Unisex",
                Brand = "SportStyle",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Navy Blue Blazer",
                Description = "Professional blazer for business attire",
                Price = 129.99m,
                ImageUrl = "https://example.com/images/navy-blazer.jpg",
                Stock = 20,
                Category = "Blazers",
                Size = "M",
                Color = "Navy",
                Material = "Wool",
                Gender = "Men",
                Brand = "BusinessWear",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Green Cargo Pants",
                Description = "Durable cargo pants with multiple pockets",
                Price = 59.99m,
                ImageUrl = "https://example.com/images/cargo-pants.jpg",
                Stock = 28,
                Category = "Pants",
                Size = "34W x 32L",
                Color = "Green",
                Material = "Cotton",
                Gender = "Men",
                Brand = "AdventureGear",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Pink Floral Skirt",
                Description = "Beautiful floral skirt for spring",
                Price = 34.99m,
                ImageUrl = "https://example.com/images/floral-skirt.jpg",
                Stock = 22,
                Category = "Skirts",
                Size = "M",
                Color = "Pink",
                Material = "Cotton",
                Gender = "Women",
                Brand = "FloralFashion",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Black Polo Shirt",
                Description = "Classic black polo shirt",
                Price = 29.99m,
                ImageUrl = "https://example.com/images/black-polo.jpg",
                Stock = 45,
                Category = "Polos",
                Size = "L",
                Color = "Black",
                Material = "Cotton",
                Gender = "Men",
                Brand = "ClassicWear",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        Console.WriteLine($"Seeding {products.Count} products...");
        await collection.InsertManyAsync(products);
        Console.WriteLine("Product seeding completed successfully!");
    }
}
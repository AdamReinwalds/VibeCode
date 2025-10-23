using ECommerceMVP.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ECommerceMVP.UnitTests.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Product_DefaultConstructor_AllowsPropertyAssignment()
    {
        // Act
        var product = new Product();

        // Assert - Entity should be created with default values
        product.Should().NotBeNull();
        product.Id.Should().BeNull();
        product.Name.Should().BeNull();
        product.Description.Should().BeNull();
        product.Price.Should().Be(0);
        product.Stock.Should().Be(0);
        product.Category.Should().BeNull();
        product.CreatedAt.Should().Be(default);
        product.UpdatedAt.Should().Be(default);
    }

    [Fact]
    public void Product_PropertyAssignment_WorksCorrectly()
    {
        // Arrange
        var product = new Product();
        var id = "507f1f77bcf86cd799439011";
        var name = "Test Product";
        var description = "Test Description";
        var price = 99.99m;
        var stock = 10;
        var category = "Electronics";
        var imageUrl = "https://example.com/image.jpg";
        var size = "Medium";
        var color = "Blue";
        var material = "Plastic";
        var gender = "Unisex";
        var brand = "TestBrand";
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var updatedAt = DateTime.UtcNow;

        // Act
        product.Id = id;
        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.Stock = stock;
        product.Category = category;
        product.ImageUrl = imageUrl;
        product.Size = size;
        product.Color = color;
        product.Material = material;
        product.Gender = gender;
        product.Brand = brand;
        product.CreatedAt = createdAt;
        product.UpdatedAt = updatedAt;

        // Assert
        product.Id.Should().Be(id);
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Should().Be(price);
        product.Stock.Should().Be(stock);
        product.Category.Should().Be(category);
        product.ImageUrl.Should().Be(imageUrl);
        product.Size.Should().Be(size);
        product.Color.Should().Be(color);
        product.Material.Should().Be(material);
        product.Gender.Should().Be(gender);
        product.Brand.Should().Be(brand);
        product.CreatedAt.Should().Be(createdAt);
        product.UpdatedAt.Should().Be(updatedAt);
    }

    [Theory]
    [InlineData("Gaming Laptop", "High-performance gaming laptop", 1299.99, 5, "Electronics")]
    [InlineData("Coffee Maker", "Programmable coffee maker", 79.99, 25, "Appliances")]
    [InlineData("Running Shoes", "Comfortable running shoes", 89.99, 50, "Sports")]
    [InlineData("Novel", "Bestselling fiction novel", 14.99, 100, "Books")]
    [InlineData("Desk Lamp", "LED desk lamp with adjustable brightness", 34.99, 30, "Furniture")]
    public void Product_Properties_CanBeSetToVariousValidValues(string name, string description, decimal price, int stock, string category)
    {
        // Arrange
        var product = new Product();

        // Act
        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.Stock = stock;
        product.Category = category;

        // Assert
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Should().Be(price);
        product.Stock.Should().Be(stock);
        product.Category.Should().Be(category);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Product_Stock_CanBeSetToVariousQuantities(int stock)
    {
        // Arrange
        var product = new Product();

        // Act
        product.Stock = stock;

        // Assert
        product.Stock.Should().Be(stock);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(9.99)]
    [InlineData(99.99)]
    [InlineData(999.99)]
    [InlineData(9999.99)]
    public void Product_Price_CanBeSetToVariousValues(decimal price)
    {
        // Arrange
        var product = new Product();

        // Act
        product.Price = price;

        // Assert
        product.Price.Should().Be(price);
    }

    [Fact]
    public void Product_ImageUrl_CanBeSetToNull()
    {
        // Arrange
        var product = new Product();

        // Act
        product.ImageUrl = null;

        // Assert
        product.ImageUrl.Should().BeNull();
    }

    [Theory]
    [InlineData("http://example.com/image.jpg")]
    [InlineData("https://example.com/image.png")]
    [InlineData("/images/product.jpg")]
    [InlineData("data:image/jpeg;base64,/9j/4AAQ")]
    public void Product_ImageUrl_CanBeSetToVariousUrls(string imageUrl)
    {
        // Arrange
        var product = new Product();

        // Act
        product.ImageUrl = imageUrl;

        // Assert
        product.ImageUrl.Should().Be(imageUrl);
    }

    [Theory]
    [InlineData("Small")]
    [InlineData("Medium")]
    [InlineData("Large")]
    [InlineData("XL")]
    [InlineData("32x32")]
    public void Product_Size_CanBeSetToVariousValues(string size)
    {
        // Arrange
        var product = new Product();

        // Act
        product.Size = size;

        // Assert
        product.Size.Should().Be(size);
    }

    [Theory]
    [InlineData("Red")]
    [InlineData("Blue")]
    [InlineData("Green")]
    [InlineData("Black")]
    [InlineData("White")]
    [InlineData("#FF0000")]
    public void Product_Color_CanBeSetToVariousValues(string color)
    {
        // Arrange
        var product = new Product();

        // Act
        product.Color = color;

        // Assert
        product.Color.Should().Be(color);
    }

    [Theory]
    [InlineData("Cotton")]
    [InlineData("Polyester")]
    [InlineData("Leather")]
    [InlineData("Wood")]
    [InlineData("Metal")]
    [InlineData("Plastic")]
    public void Product_Material_CanBeSetToVariousValues(string material)
    {
        // Arrange
        var product = new Product();

        // Act
        product.Material = material;

        // Assert
        product.Material.Should().Be(material);
    }

    [Theory]
    [InlineData("Male")]
    [InlineData("Female")]
    [InlineData("Unisex")]
    [InlineData("Kids")]
    public void Product_Gender_CanBeSetToVariousValues(string gender)
    {
        // Arrange
        var product = new Product();

        // Act
        product.Gender = gender;

        // Assert
        product.Gender.Should().Be(gender);
    }

    [Theory]
    [InlineData("Nike")]
    [InlineData("Apple")]
    [InlineData("Samsung")]
    [InlineData("Generic")]
    [InlineData("BrandXYZ")]
    public void Product_Brand_CanBeSetToVariousValues(string brand)
    {
        // Arrange
        var product = new Product();

        // Act
        product.Brand = brand;

        // Assert
        product.Brand.Should().Be(brand);
    }

    [Fact]
    public void Product_CreatedAt_CanBeSetToPastDate()
    {
        // Arrange
        var product = new Product();
        var pastDate = DateTime.UtcNow.AddDays(-30);

        // Act
        product.CreatedAt = pastDate;

        // Assert
        product.CreatedAt.Should().Be(pastDate);
    }

    [Fact]
    public void Product_CreatedAt_CanBeSetToFutureDate()
    {
        // Arrange
        var product = new Product();
        var futureDate = DateTime.UtcNow.AddDays(30);

        // Act
        product.CreatedAt = futureDate;

        // Assert
        product.CreatedAt.Should().Be(futureDate);
    }

    [Fact]
    public void Product_UpdatedAt_CanBeSetToCurrentTime()
    {
        // Arrange
        var product = new Product();
        var currentTime = DateTime.UtcNow;

        // Act
        product.UpdatedAt = currentTime;

        // Assert
        product.UpdatedAt.Should().Be(currentTime);
    }

    [Fact]
    public void Product_Id_CanBeSetToValidObjectId()
    {
        // Arrange
        var product = new Product();
        var validObjectId = "507f1f77bcf86cd799439011";

        // Act
        product.Id = validObjectId;

        // Assert
        product.Id.Should().Be(validObjectId);
    }

    [Theory]
    [InlineData("Electronics")]
    [InlineData("Clothing")]
    [InlineData("Home & Garden")]
    [InlineData("Sports & Outdoors")]
    [InlineData("Books")]
    [InlineData("Toys & Games")]
    public void Product_Category_CanBeSetToVariousCategories(string category)
    {
        // Arrange
        var product = new Product();

        // Act
        product.Category = category;

        // Assert
        product.Category.Should().Be(category);
    }

    [Theory]
    [InlineData("This is a short description.")]
    [InlineData("This is a much longer description that provides more details about the product and its features.")]
    [InlineData("A")]
    [InlineData("Product description with special characters: @#$%^&*()")]
    [InlineData("Description with numbers: 123456789")]
    public void Product_Description_CanBeSetToVariousLengths(string description)
    {
        // Arrange
        var product = new Product();

        // Act
        product.Description = description;

        // Assert
        product.Description.Should().Be(description);
    }

    [Theory]
    [InlineData("Product Name")]
    [InlineData("A")]
    [InlineData("Very Long Product Name That Might Be Used For Some Products")]
    [InlineData("Product with numbers: 123")]
    [InlineData("Product with special chars: @#$")]
    public void Product_Name_CanBeSetToVariousValues(string name)
    {
        // Arrange
        var product = new Product();

        // Act
        product.Name = name;

        // Assert
        product.Name.Should().Be(name);
    }

    [Fact]
    public void Product_AllProperties_CanBeSetSimultaneously()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Id = "507f1f77bcf86cd799439011";
        product.Name = "Test Product";
        product.Description = "Test Description";
        product.Price = 99.99m;
        product.Stock = 10;
        product.Category = "Electronics";
        product.ImageUrl = "https://example.com/image.jpg";
        product.Size = "Medium";
        product.Color = "Blue";
        product.Material = "Plastic";
        product.Gender = "Unisex";
        product.Brand = "TestBrand";
        product.CreatedAt = DateTime.UtcNow.AddDays(-1);
        product.UpdatedAt = DateTime.UtcNow;

        // Assert - All properties should be set correctly
        product.Id.Should().Be("507f1f77bcf86cd799439011");
        product.Name.Should().Be("Test Product");
        product.Description.Should().Be("Test Description");
        product.Price.Should().Be(99.99m);
        product.Stock.Should().Be(10);
        product.Category.Should().Be("Electronics");
        product.ImageUrl.Should().Be("https://example.com/image.jpg");
        product.Size.Should().Be("Medium");
        product.Color.Should().Be("Blue");
        product.Material.Should().Be("Plastic");
        product.Gender.Should().Be("Unisex");
        product.Brand.Should().Be("TestBrand");
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(-1), TimeSpan.FromSeconds(1));
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Product_DefaultValues_AreCorrect()
    {
        // Act
        var product = new Product();

        // Assert
        product.Id.Should().BeNull();
        product.Name.Should().BeNull();
        product.Description.Should().BeNull();
        product.Price.Should().Be(0);
        product.ImageUrl.Should().BeNull();
        product.Stock.Should().Be(0);
        product.Category.Should().BeNull();
        product.Size.Should().BeNull();
        product.Color.Should().BeNull();
        product.Material.Should().BeNull();
        product.Gender.Should().BeNull();
        product.Brand.Should().BeNull();
        product.CreatedAt.Should().Be(default);
        product.UpdatedAt.Should().Be(default);
    }

    [Fact]
    public void Product_TwoInstances_HaveIndependentProperties()
    {
        // Arrange
        var product1 = new Product();
        var product2 = new Product();

        // Act
        product1.Name = "Product 1";
        product1.Stock = 10;
        product2.Name = "Product 2";
        product2.Stock = 20;

        // Assert
        product1.Name.Should().Be("Product 1");
        product1.Stock.Should().Be(10);
        product2.Name.Should().Be("Product 2");
        product2.Stock.Should().Be(20);
    }
}
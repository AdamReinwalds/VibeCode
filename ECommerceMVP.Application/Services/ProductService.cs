using ECommerceMVP.Application.DTOs;
using ECommerceMVP.Application.Services.Interfaces;
using ECommerceMVP.Domain.Entities;
using ECommerceMVP.Domain.Repositories;

namespace ECommerceMVP.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        // Get all products by setting a high limit and page 1
        var products = await _productRepository.GetAllAsync(page: 1, limit: int.MaxValue);
        return products.Select(ToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(string id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product != null ? ToDto(product) : null;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            ImageUrl = createProductDto.ImageUrl,
            Stock = createProductDto.Stock,
            Category = createProductDto.Category,
            Size = createProductDto.Size,
            Color = createProductDto.Color,
            Material = createProductDto.Material,
            Gender = createProductDto.Gender,
            Brand = createProductDto.Brand,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _productRepository.CreateAsync(product);
        return ToDto(product);
    }

    public async Task<bool> UpdateAsync(string id, UpdateProductDto updateProductDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return false;

        if (updateProductDto.Name != null) product.Name = updateProductDto.Name;
        if (updateProductDto.Description != null) product.Description = updateProductDto.Description;
        if (updateProductDto.Price.HasValue) product.Price = updateProductDto.Price.Value;
        if (updateProductDto.ImageUrl != null) product.ImageUrl = updateProductDto.ImageUrl;
        if (updateProductDto.Stock.HasValue) product.Stock = updateProductDto.Stock.Value;
        if (updateProductDto.Category != null) product.Category = updateProductDto.Category;
        if (updateProductDto.Size != null) product.Size = updateProductDto.Size;
        if (updateProductDto.Color != null) product.Color = updateProductDto.Color;
        if (updateProductDto.Material != null) product.Material = updateProductDto.Material;
        if (updateProductDto.Gender != null) product.Gender = updateProductDto.Gender;
        if (updateProductDto.Brand != null) product.Brand = updateProductDto.Brand;

        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        await _productRepository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category)
    {
        var products = await _productRepository.GetAllAsync(category: category);
        return products.Select(ToDto);
    }

    private static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Stock = product.Stock,
            Category = product.Category,
            Size = product.Size,
            Color = product.Color,
            Material = product.Material,
            Gender = product.Gender,
            Brand = product.Brand,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
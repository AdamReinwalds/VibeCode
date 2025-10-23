using ECommerceMVP.Application.DTOs;

namespace ECommerceMVP.Application.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(string id);
    Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
    Task<bool> UpdateAsync(string id, UpdateProductDto updateProductDto);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category);
}
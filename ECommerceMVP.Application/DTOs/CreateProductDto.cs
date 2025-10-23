namespace ECommerceMVP.Application.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = null!;
    public string Size { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string Material { get; set; } = null!;
    public string Gender { get; set; } = null!;
    public string Brand { get; set; } = null!;
}
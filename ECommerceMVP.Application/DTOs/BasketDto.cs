namespace ECommerceMVP.Application.DTOs;

public class BasketDto
{
    public string BasketId { get; set; } = null!;
    public List<BasketItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

public class BasketItemDto
{
    public string ProductId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace ECommerceMVP.Application.DTOs;

public class AddToBasketRequest
{
    [Required]
    public string ProductId { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}
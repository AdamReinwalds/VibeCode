using System.ComponentModel.DataAnnotations;

namespace ECommerceMVP.Application.DTOs;

public class CheckoutRequest
{
    [Required]
    public ShippingAddressDto ShippingAddress { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = null!;
}

public class ShippingAddressDto
{
    [Required]
    [StringLength(200)]
    public string Street { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string PostalCode { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = null!;
}
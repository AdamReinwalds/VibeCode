using System.ComponentModel.DataAnnotations;

namespace ECommerceMVP.Application.DTOs;

public class RegisterRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = null!;
}
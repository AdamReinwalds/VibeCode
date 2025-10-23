namespace ECommerceMVP.Application.DTOs;

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public UserResponse User { get; set; } = null!;
}

public class UserResponse
{
    public string UserId { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
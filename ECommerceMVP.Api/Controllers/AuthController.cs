using ECommerceMVP.Application.DTOs;
using ECommerceMVP.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVP.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = new { code = "VALIDATION_ERROR", message = "Invalid request data" } });

            var response = await _authService.RegisterAsync(request);
            return Created("", response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = new { code = "CONFLICT", message = ex.Message } });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "An error occurred during registration" } });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = new { code = "VALIDATION_ERROR", message = "Invalid request data" } });

            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { error = new { code = "UNAUTHORIZED", message = ex.Message } });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "An error occurred during login" } });
        }
    }
}
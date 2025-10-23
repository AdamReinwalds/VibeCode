using ECommerceMVP.Application.DTOs;
using ECommerceMVP.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceMVP.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public OrdersController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        try
        {
            if (page < 1 || limit < 1 || limit > 100)
                return BadRequest(new { error = new { code = "VALIDATION_ERROR", message = "Invalid pagination parameters" } });

            var userId = GetUserId();
            var orders = await _checkoutService.GetUserOrdersAsync(userId, page, limit);

            return Ok(orders);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to retrieve orders" } });
        }
    }

    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID not found in token");
        return userId;
    }
}
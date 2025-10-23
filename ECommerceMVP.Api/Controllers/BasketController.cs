using ECommerceMVP.Application.DTOs;
using ECommerceMVP.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceMVP.Api.Controllers;

[ApiController]
[Route("api/basket")]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBasket()
    {
        try
        {
            var userId = GetUserId();
            var basket = await _basketService.GetBasketAsync(userId);
            return Ok(basket);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to retrieve basket" } });
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToBasket([FromBody] AddToBasketRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = new { code = "VALIDATION_ERROR", message = "Invalid request data" } });

            var userId = GetUserId();
            var result = await _basketService.AddToBasketAsync(userId, request);
            return Ok(new { basketItem = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = new { code = "VALIDATION_ERROR", message = ex.Message } });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to add item to basket" } });
        }
    }

    [HttpDelete("remove/{productId}")]
    public async Task<IActionResult> RemoveFromBasket(string productId)
    {
        try
        {
            var userId = GetUserId();
            await _basketService.RemoveFromBasketAsync(userId, productId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = new { code = "VALIDATION_ERROR", message = ex.Message } });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to remove item from basket" } });
        }
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearBasket()
    {
        try
        {
            var userId = GetUserId();
            await _basketService.ClearBasketAsync(userId);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Failed to clear basket" } });
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
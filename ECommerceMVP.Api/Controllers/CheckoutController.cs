using ECommerceMVP.Application.DTOs;
using ECommerceMVP.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceMVP.Api.Controllers;

[ApiController]
[Route("api/checkout")]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpPost]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = new { code = "VALIDATION_ERROR", message = "Invalid request data" } });

            var userId = GetUserId();
            var orderSummary = await _checkoutService.CheckoutAsync(userId, request);

            return Created("", orderSummary);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = new { code = "CHECKOUT_ERROR", message = ex.Message } });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = new { code = "INTERNAL_ERROR", message = "Checkout failed" } });
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
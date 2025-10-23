using ECommerceMVP.Application.DTOs;

namespace ECommerceMVP.Application.Services.Interfaces;

public interface ICheckoutService
{
    Task<OrderSummaryDto> CheckoutAsync(string userId, CheckoutRequest request);
    Task<OrdersResponse> GetUserOrdersAsync(string userId, int page = 1, int limit = 10);
}
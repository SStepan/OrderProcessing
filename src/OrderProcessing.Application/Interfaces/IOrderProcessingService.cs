using OrderProcessing.Application.DTOs;

namespace OrderProcessing.Application.Interfaces;

public interface IOrderProcessingService
{
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<bool> ProcessOrderAsync(Guid orderId);
}
namespace OrderProcessing.Application.DTOs;

public class CreateOrderRequest
{
    public long CustomerId { get; set; }
    public List<OrderItemRequest> Items { get; set; } = new();
}
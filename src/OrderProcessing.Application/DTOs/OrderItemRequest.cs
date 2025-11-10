namespace OrderProcessing.Application.DTOs;

public class OrderItemRequest
{
    public long ProductId { get; set; }
    public int Qty { get; set; }
}
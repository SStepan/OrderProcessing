namespace OrderProcessing.Application.DTOs;

public class CreateOrderResponse
{
    public Guid OrderId { get; set; }
    public string Status { get; set; }
    public decimal TotalCents { get; set; }
}
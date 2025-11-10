using OrderProcessing.Core.Enums;

namespace OrderProcessing.Core.Entities;

public class Order
{
    public Guid OrderId { get; set; }
    public long CustomerId { get; set; }
    public int TotalCents { get; set; }
    public string Status { get; set; } = "pending"; 
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ProcessedAt { get; set; }
    public string? ProcessingNotes { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
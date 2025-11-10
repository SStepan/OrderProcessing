using System.Reflection.Metadata.Ecma335;

namespace OrderProcessing.Core.Entities;

public class OrderItem
{
    public long OrderItemId { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } 

    public long ProductId { get; set; }
    public int Qty { get; set; }
    public int UnitPriceCents { get; set; }
    
}
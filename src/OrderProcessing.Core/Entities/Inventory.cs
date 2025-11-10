namespace OrderProcessing.Core.Entities;

public class Inventory
{
    public long ProductId { get; set; }
    public string Sku { get; set; } = "";
    public int UnitPriceCents { get; set; }
    public int Qty { get; set; }
}
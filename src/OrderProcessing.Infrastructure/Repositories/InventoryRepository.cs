using Microsoft.EntityFrameworkCore;
using OrderProcessing.Core.Entities;
using OrderProcessing.Core.Interfaces;
using OrderProcessing.Infrastructure.Data;

namespace OrderProcessing.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly OrdersDbContext _context;
    
    public InventoryRepository(OrdersDbContext context)
    {
        _context = context;
    }
    
    public async Task<Inventory?> GetByProductIdAsync(long productId)
    {
        return await _context.Inventory.FindAsync(productId);
    }
    
    public async Task<bool> TryReserveAsync(long productId, int qty)
    {
        var rows = await _context.Database.ExecuteSqlRawAsync(@"
            UPDATE ""Inventories""
            SET ""Qty"" = ""Qty"" - {0}
            WHERE ""ProductId"" = {1} AND ""Qty"" >= {0}",
            qty, productId);
        
        return rows > 0;
    }
}
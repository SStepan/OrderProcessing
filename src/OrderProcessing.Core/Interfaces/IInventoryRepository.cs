using OrderProcessing.Core.Entities;

namespace OrderProcessing.Core.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory?> GetByProductIdAsync(long productId);
    Task<bool> TryReserveAsync(long productId, int qty);
}
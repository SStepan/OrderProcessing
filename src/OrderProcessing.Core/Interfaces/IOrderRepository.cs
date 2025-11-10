using OrderProcessing.Core.Entities;

namespace OrderProcessing.Core.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetTopOrdersAsync(int count);
    Task<Order?> GetByIdAsync(Guid orderId);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}
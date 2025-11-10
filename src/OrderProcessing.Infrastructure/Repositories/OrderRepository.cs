using Microsoft.EntityFrameworkCore;
using OrderProcessing.Core.Entities;
using OrderProcessing.Core.Interfaces;
using OrderProcessing.Infrastructure.Data;

namespace OrderProcessing.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _context;
    
    public OrderRepository(OrdersDbContext context)
    {
        _context = context;
    }
    
    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }
    
    public async Task<IEnumerable<Order>> GetTopOrdersAsync(int count)
    {

        return await _context.Orders
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
    
    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}
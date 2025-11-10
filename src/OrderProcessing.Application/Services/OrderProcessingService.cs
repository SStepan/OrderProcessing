using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Core.Entities;
using OrderProcessing.Core.Enums;
using OrderProcessing.Core.Interfaces;
using OrderProcessing.Infrastructure.Data;

namespace OrderProcessing.Application.Services;

public class OrderProcessingService : IOrderProcessingService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IInventoryRepository _inventoryRepo;
    private readonly ILogger<OrderProcessingService> _logger;
    private readonly OrdersDbContext _context;
    
    public OrderProcessingService(
        IOrderRepository orderRepo,
        IInventoryRepository inventoryRepo,
        ILogger<OrderProcessingService> logger, OrdersDbContext context)
    {
        _orderRepo = orderRepo;
        _inventoryRepo = inventoryRepo;
        _logger = logger;
        _context = context;
    }
    
    public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        // TODO: Fluent validators
        if (request.CustomerId <= 0)
            throw new ArgumentException("Invalid customer ID");
        
        if (!request.Items.Any())
            throw new ArgumentException("Order must have items");
        
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Status = OrderStatus.Pending,
        };
        
        foreach (var item in request.Items)
        {
            var product = await _inventoryRepo.GetByProductIdAsync(item.ProductId);
            if (product == null)
                throw new ArgumentException($"Product {item.ProductId} not found");
            
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Qty = item.Qty,
                UnitPriceCents = product.UnitPriceCents
            });
        }
        
        order.TotalCents = order.Items.Sum(i => i.Qty * i.UnitPriceCents);
        
        if (order.TotalCents > 10000) // $100.00
        {
            order.TotalCents = (int)(order.TotalCents * (decimal)0.9);
        }
        
        await _orderRepo.AddAsync(order);
        
        _logger.LogInformation("Created order {OrderId} for customer {CustomerId}", 
            order.OrderId, order.CustomerId);
        
        return new CreateOrderResponse
        {
            OrderId = order.OrderId,
            Status = order.Status,
            TotalCents = order.TotalCents
        };
    }
    
    public async Task<bool> ProcessOrderAsync(Guid orderId)
    {
        try
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                return false;
            }
        
            if (order.Status != OrderStatus.Pending)
            {
                _logger.LogInformation("Order {OrderId} already {Status}", orderId, order.Status.ToString());
                return order.Status == OrderStatus.Processed;
            }
        
            foreach (var item in order.Items)
            {
                var reserved = await _inventoryRepo.TryReserveAsync(item.ProductId, item.Qty);
                if (!reserved)
                {
                    order.Status = OrderStatus.Failed;
                    order.ProcessingNotes = $"Out of stock: product {item.ProductId}";
                    order.ProcessedAt = DateTimeOffset.UtcNow;
                    await _orderRepo.UpdateAsync(order);
                
                    _logger.LogWarning("Order {OrderId} failed: {Notes}", orderId, order.ProcessingNotes);
                    return false;
                }
            }
        
            order.Status = OrderStatus.Processed;
            order.ProcessedAt = DateTimeOffset.UtcNow;
            await _orderRepo.UpdateAsync(order);
        
            _logger.LogInformation("Order {OrderId} processed successfully", orderId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing order {OrderId}", orderId);
        }

        return true;
    }
}
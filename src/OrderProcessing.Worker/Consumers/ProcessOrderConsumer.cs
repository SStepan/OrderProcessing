
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Contracts;
using OrderProcessing.Core.Enums;
using OrderProcessing.Infrastructure.Data;

namespace Orders.Worker.Consumers;

public class ProcessOrderConsumer : IConsumer<OrderSubmittedMessage>
{
    private readonly ILogger<ProcessOrderConsumer> _log;
    
    private readonly IOrderProcessingService  _orderProcessingService;

    private static long _processedCount;

    public ProcessOrderConsumer(ILogger<ProcessOrderConsumer> log, IOrderProcessingService orderProcessingService)
    {
        _log = log;
        _orderProcessingService = orderProcessingService;
    }

    public async Task Consume(ConsumeContext<OrderSubmittedMessage> ctx)
    {
        var orderId = ctx.Message.OrderId;
        await _orderProcessingService.ProcessOrderAsync(orderId);
        
        Interlocked.Increment(ref _processedCount);
        
        _log.LogInformation($"Processed order {orderId}. Total: {_processedCount}");
    }
}
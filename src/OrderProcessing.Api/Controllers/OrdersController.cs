// src/OrderProcessing.Api/Controllers/OrdersController.cs

using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Services;
using OrderProcessing.Contracts;
using OrderProcessing.Core.Interfaces;

namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderProcessingService _orderService;

    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrdersController> _logger;
    private readonly IOrderRepository _orderRepository;
    
    public OrdersController(
        IOrderProcessingService orderService,
        IPublishEndpoint publishEndpoint,
        ILogger<OrdersController> logger, IOrderRepository orderRepository)
    {
        _orderService = orderService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _orderRepository = orderRepository;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        var response = await _orderService.CreateOrderAsync(request);
        
        // Publish to queue
        await _publishEndpoint.Publish<OrderSubmittedMessage>(new OrderSubmittedMessage
        {
            OrderId = response.OrderId
        });
        
        _logger.LogInformation("Order {OrderId} queued for processing", response.OrderId);
        
        return Accepted(response);
    }
    
    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(int count = 10)
    {
        var orders = await _orderRepository.GetTopOrdersAsync(count);
        return Ok(orders);
    }
}
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Services;
using OrderProcessing.Core.Interfaces;
using OrderProcessing.Infrastructure.Data;
using OrderProcessing.Infrastructure.Repositories;
using Orders.Worker.Consumers;

var b = Host.CreateApplicationBuilder(args);

b.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(b.Configuration.GetConnectionString("Default")));

b.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProcessOrderConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        var host = b.Configuration["Rabbit:Host"] ?? "rabbitmq";
        var user = b.Configuration["Rabbit:User"] ?? "guest";
        var pass = b.Configuration["Rabbit:Pass"] ?? "guest";

        cfg.Host(host, "/", h => { h.Username(user); h.Password(pass); });

        cfg.ConfigureEndpoints(ctx);
    });
});

b.Services.AddScoped<IOrderProcessingService, OrderProcessingService>();
b.Services.AddScoped<IOrderRepository, OrderRepository>();
b.Services.AddScoped<IInventoryRepository, InventoryRepository>();

var app = b.Build();

await app.RunAsync();
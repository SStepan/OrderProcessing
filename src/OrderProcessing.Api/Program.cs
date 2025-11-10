using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Services;
using OrderProcessing.Contracts;
using OrderProcessing.Core.Entities;
using OrderProcessing.Core.Enums;
using OrderProcessing.Core.Interfaces;
using OrderProcessing.Infrastructure.Data;
using OrderProcessing.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrdersDbContext>(o =>
{
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        
        cfg.Host(builder.Configuration["Rabbit:Host"], "/", h =>
        {
            h.Username(builder.Configuration["Rabbit:User"]);
            h.Password(builder.Configuration["Rabbit:Pass"]);
        });
    });
});

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IOrderProcessingService, OrderProcessingService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Database.Migrate();
    if (!db.Inventory.Any())
    {
        db.Inventory.AddRange(
            new Inventory { ProductId = 1, Sku = "SKU-1", UnitPriceCents = 1999, Qty = 10 },
            new Inventory { ProductId = 2, Sku = "SKU-2", UnitPriceCents = 499,  Qty = 50 }
        );
        db.SaveChanges();
    }
}

app.MapControllers();
app.Run();




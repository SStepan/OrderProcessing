using Microsoft.EntityFrameworkCore;
using OrderProcessing.Core.Entities;

namespace OrderProcessing.Infrastructure.Data;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) 
        : base(options) { }
    
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Inventory> Inventory => Set<Inventory>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.ProcessingNotes).HasMaxLength(500);
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Status);
        });
        
        // OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");
            entity.HasKey(e => e.OrderItemId);
            entity.HasOne(e => e.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // InventoryItem
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("Inventories");
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.Sku).HasMaxLength(50);
        });
    }
}
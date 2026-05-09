using Microsoft.EntityFrameworkCore;
using Pedido.Domain.Entities;

namespace Pedido.Infrastructure.Data;

public sealed class PedidoDbContext : DbContext
{
    public PedidoDbContext(DbContextOptions<PedidoDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasMany(e => e.Items).WithOne().HasForeignKey(e => e.OrderId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}

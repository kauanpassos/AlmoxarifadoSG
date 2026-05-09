using Microsoft.EntityFrameworkCore;
using Estoque.Domain.Entities;

namespace Estoque.Infrastructure.Data;

public sealed class EstoqueDbContext : DbContext
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockMovement> Movements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Quantity).IsRequired();
        });

        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.IdempotencyKey).IsUnique();
        });
    }
}

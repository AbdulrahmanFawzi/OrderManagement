using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductId)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .IsRequired();

        // Historical snapshot: UnitPrice is copied from Product.Price
        // at add-time and never updated afterwards.
        builder.Property(i => i.UnitPrice)
            .IsRequired()
            .HasColumnType("NUMBER(18,2)");

        // Product 1 -> * OrderItem (no cascade: order history survives
        // even if a product is later removed/deactivated).
        // The FK to Product is informational; the snapshot price
        // means OrderItem does not depend on Product at read time.
    }
}

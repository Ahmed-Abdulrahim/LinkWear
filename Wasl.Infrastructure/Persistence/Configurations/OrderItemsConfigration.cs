
namespace Wasl.Infrastructure.Persistence.Configurations
{
    public class OrderItemsConfigration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity)
                   .IsRequired();
            builder.Property(x => x.UnitPrice)
                  .IsRequired().HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Order)
                   .WithMany(o => o.Items)
                   .HasForeignKey(x => x.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
                   .WithMany(p => p.OrderItems)
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.OrderId, x.ProductId })
                   .IsUnique();
        }
    }
}

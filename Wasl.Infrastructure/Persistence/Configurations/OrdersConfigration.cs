namespace Wasl.Infrastructure.Persistence.Configurations
{
       public class OrdersConfigration : IEntityTypeConfiguration<Order>
       {
              public void Configure(EntityTypeBuilder<Order> builder)
              {
                     builder.HasKey(x => x.Id);

                     builder.Property(x => x.OrderNumber)
                            .IsRequired()
                            .HasMaxLength(50);

                     builder.HasIndex(x => x.OrderNumber)
                            .IsUnique();

                     builder.Property(x => x.Status)
                            .IsRequired()
                            .HasConversion<string>();

                     builder.Property(x => x.TrackingNumber)
                            .HasMaxLength(100);

                     builder.Property(x => x.TotalAmount)
                            .IsRequired()
                            .HasColumnType("decimal(18,2)");

                     builder.Property(x => x.CreatedAt)
                            .HasDefaultValueSql("GETUTCDATE()");

                     builder.HasOne(x => x.StoreOwner)
                            .WithMany(u => u.StoreOrders)
                            .HasForeignKey(x => x.StoreOwnerId)
                            .OnDelete(DeleteBehavior.Restrict);

                     builder.HasOne(x => x.Supplier)
                            .WithMany(u => u.SupplierOrders)
                            .HasForeignKey(x => x.SupplierId)
                            .OnDelete(DeleteBehavior.Restrict);

                     builder.HasIndex(x => x.StoreOwnerId);
                     builder.HasIndex(x => x.SupplierId);
                     builder.HasIndex(x => x.Status);
              }
       }
}

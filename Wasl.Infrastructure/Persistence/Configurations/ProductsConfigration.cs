namespace Wasl.Infrastructure.Persistence.Configurations
{
       public class ProductsConfigration : IEntityTypeConfiguration<Product>
       {
              public void Configure(EntityTypeBuilder<Product> builder)
              {
                     builder.HasKey(x => x.Id);

                     builder.Property(x => x.Name)
                            .IsRequired()
                            .HasMaxLength(200);

                     builder.Property(x => x.Price)
                            .HasColumnType("decimal(18,2)")
                            .IsRequired();
                     builder.Property(x => x.AvailableQuantity)
                         .IsRequired();

                     builder.Property(x => x.MinimumOrder)
                         .IsRequired(false);




                     builder.Property(x => x.CreatedAt)
                            .HasDefaultValueSql("GETUTCDATE()");

                     builder.HasOne(x => x.Supplier)
                            .WithMany(u => u.Products)
                            .HasForeignKey(x => x.SupplierId)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasIndex(x => x.SupplierId);
              }
       }
}

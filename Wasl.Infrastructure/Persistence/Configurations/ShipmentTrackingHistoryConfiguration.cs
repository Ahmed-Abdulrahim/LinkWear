namespace Wasl.Infrastructure.Persistence.Configurations
{
    public class ShipmentTrackingHistoryConfiguration : IEntityTypeConfiguration<ShipmentTrackingHistory>
    {
        public void Configure(EntityTypeBuilder<ShipmentTrackingHistory> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.StatusDescription)
                   .HasMaxLength(500);

            builder.Property(x => x.StatusDate)
                   .IsRequired();

            builder.HasOne(x => x.Order)
                   .WithMany(o => o.TrackingHistory)
                   .HasForeignKey(x => x.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.IsCurrent);
        }
    }
}

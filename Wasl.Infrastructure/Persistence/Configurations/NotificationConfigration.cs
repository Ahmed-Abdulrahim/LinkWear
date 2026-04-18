
namespace Wasl.Infrastructure.Persistence.Configurations
{
       public class NotificationConfigration : IEntityTypeConfiguration<Notifications>
       {
              public void Configure(EntityTypeBuilder<Notifications> builder)
              {
                     builder.HasKey(x => x.Id);

                     builder.Property(x => x.Title)
                            .IsRequired()
                            .HasMaxLength(200);


                     builder.Property(x => x.Body)
                            .IsRequired()
                            .HasMaxLength(1000);

                     builder.Property(x => x.Type)
                            .IsRequired()
                            .HasConversion<string>();

                     builder.Property(x => x.CreatedAt)
                            .HasDefaultValueSql("GETUTCDATE()");

                     builder.HasOne(x => x.User)
                            .WithMany(u => u.Notifications)
                            .HasForeignKey(x => x.UserId)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasOne(x => x.Order)
                            .WithMany()
                            .HasForeignKey(x => x.OrderId)
                            .OnDelete(DeleteBehavior.SetNull);

                     builder.HasIndex(x => x.UserId);
                     builder.HasIndex(x => x.IsRead);
                     builder.HasIndex(x => x.Type);
              }
       }
}

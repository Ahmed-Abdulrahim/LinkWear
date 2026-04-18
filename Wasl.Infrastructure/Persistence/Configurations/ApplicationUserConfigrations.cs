namespace Wasl.Infrastructure.Persistence.Configurations
{
       public class ApplicationUserConfigrations : IEntityTypeConfiguration<ApplicationUser>
       {
              public void Configure(EntityTypeBuilder<ApplicationUser> builder)
              {


                     builder.Property(x => x.BusinessName)
                            .HasMaxLength(200);
                     builder.Property(x => x.City)
                     .HasMaxLength(200);
                     builder.Property(x => x.BusinessDescription)
                     .HasMaxLength(200);

                     builder.Property(x => x.UserType)
                            .HasConversion<string>().IsRequired();
                     builder.Property(x => x.ApprovalStatus)
                           .HasConversion<string>().IsRequired();
                     builder.Property(x => x.ActivityType)
                                                .HasConversion<string>().IsRequired();

                     builder.Property(x => x.IsActive)
                            .HasDefaultValue(true);

                     builder.Property(x => x.CreatedAt)
                            .HasDefaultValueSql("GETUTCDATE()");
              }
       }
}

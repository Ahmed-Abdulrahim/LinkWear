namespace Wasl.Infrastructure.Persistence.Configurations
{
    public class FcmTokenConfiguration : IEntityTypeConfiguration<FcmToken>
    {
        public void Configure(EntityTypeBuilder<FcmToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.User)
                   .WithMany(u => u.FcmToken)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Token)
                   .IsUnique();
        }
    }
}

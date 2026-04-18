namespace Wasl.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfigration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.Property(r => r.Token).HasMaxLength(500).IsRequired();
            builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Ignore(r => r.IsExpired);

            //RelationShip
            builder.HasOne(r => r.ApplicationUser).WithMany(a => a.RefreshTokens).HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //Index
            builder.HasIndex(r => r.Token).IsUnique();
            builder.HasIndex(r => r.UserId).HasDatabaseName("IX_RefreshToken_UserId");
            builder.HasIndex(r => r.ExpiresAt).HasDatabaseName("IX_RefreshToken_ExpireAt");

        }
    }
}

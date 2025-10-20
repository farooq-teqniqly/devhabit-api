using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.Api.Database.Configurations
{
  internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
  {
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
      builder.HasKey(rt => rt.Id);
      builder.Property(rt => rt.UserId).HasMaxLength(450);
      builder.Property(rt => rt.Token).HasMaxLength(1000);
      builder.HasIndex(rt => rt.Token).IsUnique();

      builder
        .HasOne(rt => rt.User)
        .WithMany()
        .HasForeignKey(rt => rt.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}

using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.Api.Database.Configurations
{
  internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
  {
    public void Configure(EntityTypeBuilder<User> builder)
    {
      builder.HasKey(u => u.Id);
      builder.Property(u => u.Id).HasMaxLength(100);
      builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
      builder.Property(u => u.IdentityId).IsRequired().HasMaxLength(100);
      builder.Property(u => u.Name).IsRequired().HasMaxLength(100);

      builder.HasIndex(u => u.Email).IsUnique();
      builder.HasIndex(u => u.IdentityId).IsUnique();
    }
  }
}

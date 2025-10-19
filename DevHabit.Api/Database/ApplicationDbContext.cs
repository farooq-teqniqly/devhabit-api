using DevHabit.Api.Database.Configurations;
using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Database
{
  public sealed class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options) { }

    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<HabitTag> HabitTags => Set<HabitTag>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      ArgumentNullException.ThrowIfNull(modelBuilder);

      modelBuilder.HasDefaultSchema(Schemas.Application);
      modelBuilder.ApplyConfiguration(new HabitConfiguration());
      modelBuilder.ApplyConfiguration(new HabitTagConfiguration());
      modelBuilder.ApplyConfiguration(new TagConfiguration());
      modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
  }
}

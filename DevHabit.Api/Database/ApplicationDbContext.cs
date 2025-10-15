using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Database
{
  public sealed class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options) { }

    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      ArgumentNullException.ThrowIfNull(modelBuilder);

      modelBuilder.HasDefaultSchema(Schemas.Application);
      modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
  }
}

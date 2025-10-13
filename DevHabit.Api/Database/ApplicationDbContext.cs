using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Database
{
  internal sealed class ApplicationDbContext : DbContext
  {
    public DbSet<Habit> Habits => Set<Habit>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.HasDefaultSchema(Schemas.Application);
      modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
  }
}

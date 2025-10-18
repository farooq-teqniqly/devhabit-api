using DevHabit.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Extensions
{
  internal static class DatabaseExtensions
  {
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
      ArgumentNullException.ThrowIfNull(app);

      using var scope = app.Services.CreateScope();

      var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

      var identityDbContext =
        scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

      await using (applicationDbContext)
      await using (identityDbContext)
      {
        try
        {
          await applicationDbContext.Database.MigrateAsync().ConfigureAwait(false);
          app.Logger.LogInformation("Application database migrations were successfully applied.");

          await identityDbContext.Database.MigrateAsync().ConfigureAwait(false);
          app.Logger.LogInformation("Identity database migrations were successfully applied.");
        }
#pragma warning disable S2139
        catch (Exception exception)
        {
          app.Logger.LogError(exception, "Database migrations were unsuccessful.");
          throw;
        }
#pragma warning restore S2139
      }
    }
  }
}

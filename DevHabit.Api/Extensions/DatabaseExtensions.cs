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
      var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

      await using (dbContext)
      {
        try
        {
          await dbContext.Database.MigrateAsync().ConfigureAwait(false);
          app.Logger.LogInformation("Database migrations were successfully applied.");
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

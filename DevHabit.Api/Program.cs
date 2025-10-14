using DevHabit.Api.Database;
using DevHabit.Api.Extensions;
using DevHabit.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder
  .Services.AddControllers(opts =>
  {
    opts.ReturnHttpNotAcceptable = true;
  })
  .AddXmlSerializerFormatters();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
{
  opts.UseSqlServer(
      builder.Configuration.GetConnectionString("devhabit-db"),
      sqlOpts =>
      {
        sqlOpts.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Application);
      }
    )
    .UseSnakeCaseNamingConvention()
    .EnableSensitiveDataLogging();
});

builder.EnrichSqlServerDbContext<ApplicationDbContext>(settings =>
{
  settings.DisableTracing = false;
  settings.DisableHealthChecks = false;
  settings.DisableRetry = false;
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();

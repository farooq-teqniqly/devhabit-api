using DevHabit.Api.Converters;
using DevHabit.Api.Database;
using DevHabit.Api.Extensions;
using DevHabit.Api.Middleware;
using DevHabit.ServiceDefaults;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder
  .Services.AddControllers(opts =>
  {
    opts.ReturnHttpNotAcceptable = true;
  })
  .AddNewtonsoftJson()
  .AddJsonOptions(opts =>
  {
    opts.JsonSerializerOptions.Converters.Add(new CaseInsensitiveStringEnumConverter());
  });

builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
{
  var connectionString =
    builder.Configuration.GetConnectionString("devhabitdb")
    ?? throw new InvalidOperationException("Database connection string was not specified.");

  opts.UseSqlServer(
      connectionString,
      sqlOpts =>
      {
        sqlOpts.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Application);
      }
    )
    .UseSnakeCaseNamingConvention();

  if (builder.Environment.IsDevelopment())
  {
    opts.EnableSensitiveDataLogging();
  }
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
app.UseExceptionHandler();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();

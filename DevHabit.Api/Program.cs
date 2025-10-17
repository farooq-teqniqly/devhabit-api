using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Habits;
using DevHabit.Api.Entities;
using DevHabit.Api.Extensions;
using DevHabit.Api.Middleware;
using DevHabit.Api.Services.Sorting;
using DevHabit.ServiceDefaults;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder
  .Services.AddControllers(opts =>
  {
    opts.ReturnHttpNotAcceptable = true;
  })
  .AddNewtonsoftJson(opts =>
  {
    opts.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
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

builder.Services.AddSingleton<SortMappingProvider>();

builder.Services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<HabitDto, Habit>>(_ =>
  HabitMappings.SortMapping
);

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

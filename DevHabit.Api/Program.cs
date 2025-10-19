using System.Text;
using DevHabit.Api.CustomMediaTypes;
using DevHabit.Api.Database;
using DevHabit.Api.Dtos.Habits;
using DevHabit.Api.Entities;
using DevHabit.Api.Extensions;
using DevHabit.Api.Middleware;
using DevHabit.Api.Services;
using DevHabit.Api.Services.Sorting;
using DevHabit.Api.Settings;
using DevHabit.ServiceDefaults;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;
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
    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
  });

builder.Services.Configure<MvcOptions>(options =>
{
  var formatter = options.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().First();

  formatter.SupportedMediaTypes.Add(ApplicationMediaTypes.DevHabitApi);
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

builder.Services.AddDbContext<ApplicationIdentityDbContext>(opts =>
{
  var connectionString =
    builder.Configuration.GetConnectionString("devhabitdb")
    ?? throw new InvalidOperationException("Database connection string was not specified.");

  opts.UseSqlServer(
      connectionString,
      sqlOpts =>
      {
        sqlOpts.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Identity);
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
  settings.DisableRetry = true;
});

builder.Services.Configure<JwtAuthOptions>(builder.Configuration.GetSection("Jwt"));

var jwtAuthOptions =
  builder.Configuration.GetSection("Jwt").Get<JwtAuthOptions>()
  ?? throw new InvalidOperationException("Jwt auth settings not specified.");

builder
  .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(opts =>
  {
    opts.TokenValidationParameters = new TokenValidationParameters
    {
      ValidIssuer = jwtAuthOptions.Issuer,
      ValidAudience = jwtAuthOptions.Audience,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.Key)),
    };
  });

builder.Services.AddAuthorization();

builder.Services.AddScoped<TokenProvider>();

builder.Services.AddSingleton<SortMappingProvider>();

builder.Services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<HabitDto, Habit>>(_ =>
  HabitMappings.SortMapping
);

builder
  .Services.AddIdentity<IdentityUser, IdentityRole>()
  .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

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

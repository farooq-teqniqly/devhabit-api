var builder = DistributedApplication.CreateBuilder(args);

var sql = builder
  .AddSqlServer("sql")
  .WithLifetime(ContainerLifetime.Persistent)
  .WithImage("mssql/server:2022-CU21-ubuntu-22.04");

var db = sql.AddDatabase("devhabitdb", "devhabit");

var appService = builder.AddAzureAppServiceEnvironment("appservice-env");

builder
  .AddProject<Projects.DevHabit_Api>("devhabit-api")
  .WithReference(db)
  .WaitFor(db)
  .WithComputeEnvironment(appService)
  .WithExternalHttpEndpoints();

await builder.Build().RunAsync();

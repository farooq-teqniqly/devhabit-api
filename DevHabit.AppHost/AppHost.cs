var builder = DistributedApplication.CreateBuilder(args);

var sql = builder
  .AddSqlServer("sql")
  .WithLifetime(ContainerLifetime.Persistent)
  .WithImage("mssql/server:2022-CU21-ubuntu-22.04");

var db = sql.AddDatabase("devhabit-db", "devhabit");

builder.AddProject<Projects.DevHabit_Api>("devhabit-api").WithReference(db).WaitFor(db);

await builder.Build().RunAsync();

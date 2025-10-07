var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DevHabit_Api>("devhabit-api");

await builder.Build().RunAsync();

var builder = DistributedApplication.CreateBuilder(args);
var appService = builder.AddAzureAppServiceEnvironment("appservice-env");

builder
  .AddProject<Projects.DevHabit_Api>("devhabit-api")
  .WithComputeEnvironment(appService)
  .WithExternalHttpEndpoints();

await builder.Build().RunAsync();

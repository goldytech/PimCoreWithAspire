var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddContainer("redis", "redis:alpine")
    .WithArgs("redis-server", "--maxmemory", "128mb", "--maxmemory-policy", "volatile-lru", "--save", "\"\"")
    .WithVolume("pimcore-demo-redis-data", "/data");
var apiService = builder.AddProject<Projects.PimCore_ApiService>("apiservice");

builder.AddProject<Projects.PimCore_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();

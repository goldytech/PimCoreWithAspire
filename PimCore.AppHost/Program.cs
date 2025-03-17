var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddContainer("redis", "redis:alpine")
    .WithArgs("redis-server", "--maxmemory", "128mb", "--maxmemory-policy", "volatile-lru", "--save", "\"\"")
    .WithVolume("pimcore-demo-redis-data", "/data");


builder.Build().Run();

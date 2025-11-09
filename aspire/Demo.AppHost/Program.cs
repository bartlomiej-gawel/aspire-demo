var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("demo-postgres")
    .WithImage("postgres:latest")
    .WithBindMount("../../../.containers/postgres", "/var/lib/postgresql/data")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("demo-db");

builder.Build().Run();
var builder = DistributedApplication.CreateBuilder(args);

// builder.AddPostgres("demo-postgres")
//     .WithImage("postgres:latest")
//     .WithBindMount("../../.containers/postgres", "/var/lib/postgresql/18/docker")
//     .WithLifetime(ContainerLifetime.Persistent);

builder.Build().Run();